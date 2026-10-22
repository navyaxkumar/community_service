import React, { useEffect, useRef, useState } from 'react';

const VIDEO_SRC =
  'https://d8j0ntlcm91z4.cloudfront.net/user_38xzZboKViGWJOttwIXH07lWA1P/hf_20260715_090628_7052d8a6-a094-4341-a4a2-ad58493a67a9.mp4';

const MAX_CAPTURE_WIDTH = 960;
const TARGET_FPS = 30;
const FRAME_DURATION = 1000 / TARGET_FPS;
const MAX_FRAMES = 180; // Safeguard against unbounded memory allocation

export const BoomerangVideoBg: React.FC = () => {
  const videoRef = useRef<HTMLVideoElement | null>(null);
  const canvasRef = useRef<HTMLCanvasElement | null>(null);

  const [isCanvasReady, setIsCanvasReady] = useState<boolean>(false);
  const [videoFailed, setVideoFailed] = useState<boolean>(false);

  useEffect(() => {
    const video = videoRef.current;
    const canvas = canvasRef.current;
    if (!video || !canvas) return;

    let isDisposed = false;
    let captureCallbackId: number | null = null;
    let playbackRafId: number | null = null;
    let lastCaptureTime = -1;
    const capturedFrames: HTMLCanvasElement[] = [];

    // Check user preference for reduced motion
    const prefersReducedMotion = window.matchMedia(
      '(prefers-reduced-motion: reduce)'
    ).matches;

    const stopCapture = () => {
      if (captureCallbackId !== null) {
        if ('cancelVideoFrameCallback' in video && typeof (video as any).cancelVideoFrameCallback === 'function') {
          (video as any).cancelVideoFrameCallback(captureCallbackId);
        } else {
          cancelAnimationFrame(captureCallbackId);
        }
        captureCallbackId = null;
      }
    };

    const stopPlayback = () => {
      if (playbackRafId !== null) {
        cancelAnimationFrame(playbackRafId);
        playbackRafId = null;
      }
    };

    const startBoomerangPlayback = () => {
      if (isDisposed || capturedFrames.length === 0 || prefersReducedMotion) {
        if (prefersReducedMotion && capturedFrames.length > 0) {
          // Render a single high quality frame for reduced motion
          const ctx = canvas.getContext('2d');
          if (ctx) {
            const firstFrame = capturedFrames[0];
            canvas.width = firstFrame.width;
            canvas.height = firstFrame.height;
            ctx.drawImage(firstFrame, 0, 0);
            setIsCanvasReady(true);
            video.pause();
          }
        }
        return;
      }

      const firstFrame = capturedFrames[0];
      canvas.width = firstFrame.width;
      canvas.height = firstFrame.height;

      const ctx = canvas.getContext('2d');
      if (!ctx) return;

      setIsCanvasReady(true);
      video.pause();

      let currentFrameIndex = 0;
      let forward = true;
      let lastFrameTime = performance.now();

      const loop = (now: number) => {
        if (isDisposed) return;

        const delta = now - lastFrameTime;

        if (delta >= FRAME_DURATION) {
          lastFrameTime = now - (delta % FRAME_DURATION);

          const frameToDraw = capturedFrames[currentFrameIndex];
          if (frameToDraw) {
            ctx.drawImage(frameToDraw, 0, 0);
          }

          if (forward) {
            if (currentFrameIndex >= capturedFrames.length - 1) {
              forward = false;
              currentFrameIndex = Math.max(0, capturedFrames.length - 2);
            } else {
              currentFrameIndex++;
            }
          } else {
            if (currentFrameIndex <= 0) {
              forward = true;
              currentFrameIndex = Math.min(1, capturedFrames.length - 1);
            } else {
              currentFrameIndex--;
            }
          }
        }

        playbackRafId = requestAnimationFrame(loop);
      };

      playbackRafId = requestAnimationFrame(loop);
    };

    const captureCurrentFrame = () => {
      if (isDisposed || video.ended || video.paused || video.readyState < 2) return;

      const currentTime = video.currentTime;
      // Deduplicate captures by timestamp
      if (Math.abs(currentTime - lastCaptureTime) >= 0.025 && capturedFrames.length < MAX_FRAMES) {
        lastCaptureTime = currentTime;

        const originalWidth = video.videoWidth || 1280;
        const originalHeight = video.videoHeight || 720;

        const scale = Math.min(1, MAX_CAPTURE_WIDTH / originalWidth);
        const targetWidth = Math.round(originalWidth * scale);
        const targetHeight = Math.round(originalHeight * scale);

        try {
          const offscreen = document.createElement('canvas');
          offscreen.width = targetWidth;
          offscreen.height = targetHeight;
          const offscreenCtx = offscreen.getContext('2d', { alpha: false });
          if (offscreenCtx) {
            offscreenCtx.drawImage(video, 0, 0, targetWidth, targetHeight);
            capturedFrames.push(offscreen);
          }
        } catch (err) {
          console.warn('Canvas frame capture error (likely CORS):', err);
          stopCapture();
          return;
        }
      }

      scheduleNextCapture();
    };

    const scheduleNextCapture = () => {
      if (isDisposed || prefersReducedMotion || video.ended || video.paused) return;

      if ('requestVideoFrameCallback' in video && typeof (video as any).requestVideoFrameCallback === 'function') {
        captureCallbackId = (video as any).requestVideoFrameCallback(() => {
          captureCurrentFrame();
        });
      } else {
        captureCallbackId = requestAnimationFrame(() => {
          captureCurrentFrame();
        });
      }
    };

    const handleVideoPlay = () => {
      if (!prefersReducedMotion) {
        scheduleNextCapture();
      }
    };

    const handleVideoEnded = () => {
      stopCapture();
      if (capturedFrames.length > 5) {
        startBoomerangPlayback();
      } else {
        // Fallback: if not enough frames captured, replay native video smoothly
        if (!prefersReducedMotion && !isDisposed) {
          video.currentTime = 0;
          video.play().catch(() => {});
        }
      }
    };

    const handleVideoError = () => {
      console.warn('Background video failed to load, falling back to static visual.');
      setVideoFailed(true);
      stopCapture();
    };

    video.addEventListener('play', handleVideoPlay);
    video.addEventListener('ended', handleVideoEnded);
    video.addEventListener('error', handleVideoError);

    // Initial attempt to play
    video.play().catch((err) => {
      console.warn('Video autoplay prevented or deferred:', err);
    });

    return () => {
      isDisposed = true;
      stopCapture();
      stopPlayback();
      video.removeEventListener('play', handleVideoPlay);
      video.removeEventListener('ended', handleVideoEnded);
      video.removeEventListener('error', handleVideoError);
      // Clean up captured frames to avoid memory leaks
      capturedFrames.length = 0;
    };
  }, []);

  return (
    <div
      className="absolute inset-0 z-0 pointer-events-none select-none overflow-hidden"
      aria-hidden="true"
    >
      <div className="relative w-full h-full scale-[1.15] origin-top overflow-hidden">
        {/* Fallback subtle minimalist backdrop if video fails */}
        {videoFailed && (
          <div className="w-full h-full bg-gradient-to-b from-[#F5F5F7] via-[#FAFAFA] to-white" />
        )}

        {/* Live video */}
        <video
          ref={videoRef}
          src={VIDEO_SRC}
          muted
          playsInline
          autoPlay
          preload="auto"
          crossOrigin="anonymous"
          className={`w-full h-full object-cover object-top transition-opacity duration-700 ${
            isCanvasReady ? 'opacity-0 absolute inset-0' : 'opacity-100'
          }`}
        />

        {/* Boomerang display canvas */}
        <canvas
          ref={canvasRef}
          className={`w-full h-full object-cover object-top transition-opacity duration-700 ${
            isCanvasReady ? 'opacity-100' : 'opacity-0 absolute inset-0 pointer-events-none'
          }`}
        />

        {/* Ambient subtle light wash to ensure WCAG text readability while preserving cinematic quality */}
        <div className="absolute inset-0 bg-white/25 backdrop-contrast-[1.02]" />
      </div>
    </div>
  );
};
