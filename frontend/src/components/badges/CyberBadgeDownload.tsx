import React, { useRef, useState } from 'react';
import { Download } from 'lucide-react';

export const CyberBadgeDownload: React.FC = () => {
  const badgeRef = useRef<SVGSVGElement>(null);
  const [isDownloading, setIsDownloading] = useState(false);
  const [downloadError, setDownloadError] = useState<string | null>(null);

  const downloadBadge = async () => {
    const svg = badgeRef.current;
    if (!svg) return;

    setIsDownloading(true);
    setDownloadError(null);
    const svgUrl = URL.createObjectURL(
      new Blob([new XMLSerializer().serializeToString(svg)], {
        type: 'image/svg+xml;charset=utf-8',
      }),
    );

    try {
      const image = new Image();
      await new Promise<void>((resolve, reject) => {
        image.onload = () => resolve();
        image.onerror = () => reject(new Error('The badge image could not be prepared.'));
        image.src = svgUrl;
      });

      const canvas = document.createElement('canvas');
      canvas.width = 1000;
      canvas.height = 1120;
      const context = canvas.getContext('2d');
      if (!context) {
        throw new Error('Your browser could not prepare the badge image.');
      }

      context.drawImage(image, 0, 0, canvas.width, canvas.height);
      const png = await new Promise<Blob>((resolve, reject) => {
        canvas.toBlob((blob) => {
          if (blob) resolve(blob);
          else reject(new Error('Your browser could not export the badge as a PNG.'));
        }, 'image/png');
      });

      const pngUrl = URL.createObjectURL(png);
      const link = document.createElement('a');
      link.href = pngUrl;
      link.download = 'digitalshield-awareness-starter.png';
      link.click();
      window.setTimeout(() => URL.revokeObjectURL(pngUrl), 1000);
    } catch (error) {
      setDownloadError(
        error instanceof Error ? error.message : 'The badge image could not be downloaded.',
      );
    } finally {
      URL.revokeObjectURL(svgUrl);
      setIsDownloading(false);
    }
  };

  return (
    <div className="space-y-3">
      <svg
        ref={badgeRef}
        xmlns="http://www.w3.org/2000/svg"
        viewBox="0 0 1000 1120"
        role="img"
        aria-label="DigitalShield Awareness Starter cyber safety badge"
        className="mx-auto block h-auto w-full max-w-[220px] rounded-xl shadow-lg"
      >
        <defs>
          <linearGradient id="badge-background" x1="0" y1="0" x2="1" y2="1">
            <stop offset="0%" stopColor="#06121f" />
            <stop offset="55%" stopColor="#102d3a" />
            <stop offset="100%" stopColor="#071522" />
          </linearGradient>
          <linearGradient id="badge-metal" x1="0" y1="0" x2="1" y2="1">
            <stop offset="0%" stopColor="#a5f3fc" />
            <stop offset="50%" stopColor="#22d3ee" />
            <stop offset="100%" stopColor="#0e7490" />
          </linearGradient>
          <radialGradient id="badge-core">
            <stop offset="0%" stopColor="#164e63" />
            <stop offset="100%" stopColor="#071522" />
          </radialGradient>
          <pattern id="badge-grid" width="48" height="48" patternUnits="userSpaceOnUse">
            <path d="M48 0H0V48" fill="none" stroke="#67e8f9" strokeOpacity=".08" />
          </pattern>
        </defs>
        <rect width="1000" height="1120" rx="48" fill="url(#badge-background)" />
        <rect x="28" y="28" width="944" height="1064" rx="34" fill="url(#badge-grid)" />
        <rect x="28" y="28" width="944" height="1064" rx="34" fill="none" stroke="#22d3ee" strokeOpacity=".65" strokeWidth="3" />
        <path d="M105 200H895M105 920H895" stroke="#22d3ee" strokeOpacity=".35" strokeWidth="2" />
        <text x="500" y="125" fill="#ecfeff" fontFamily="Arial, sans-serif" fontSize="58" fontWeight="700" letterSpacing="7" textAnchor="middle">
          DIGITALSHIELD
        </text>
        <text x="500" y="170" fill="#67e8f9" fontFamily="Arial, sans-serif" fontSize="20" fontWeight="600" letterSpacing="6" textAnchor="middle">
          CYBER SAFETY ACHIEVEMENT
        </text>
        <polygon points="500,225 805,325 772,598 500,790 228,598 195,325" fill="#0b202e" stroke="url(#badge-metal)" strokeLinejoin="round" strokeWidth="12" />
        <polygon points="500,255 774,345 744,578 500,752 256,578 226,345" fill="none" stroke="#67e8f9" strokeOpacity=".42" strokeLinejoin="round" strokeWidth="3" />
        <path d="M500 285L708 354L687 547L500 681L313 547L292 354Z" fill="url(#badge-core)" />
        <circle cx="500" cy="465" r="118" fill="#06121f" stroke="url(#badge-metal)" strokeWidth="8" />
        <circle cx="500" cy="465" r="99" fill="none" stroke="#67e8f9" strokeOpacity=".4" strokeDasharray="9 12" strokeWidth="3" />
        <path d="M500 380L558 401V455C558 498 533 533 500 552C467 533 442 498 442 455V401L500 380Z" fill="#0e7490" stroke="#a5f3fc" strokeWidth="5" />
        <path d="M472 462L492 482L531 439" fill="none" stroke="#ecfeff" strokeLinecap="round" strokeLinejoin="round" strokeWidth="13" />
        <text x="500" y="845" fill="#ecfeff" fontFamily="Arial, sans-serif" fontSize="42" fontWeight="700" letterSpacing="2" textAnchor="middle">
          AWARENESS STARTER
        </text>
        <text x="500" y="890" fill="#a5f3fc" fontFamily="Arial, sans-serif" fontSize="21" fontWeight="500" letterSpacing="3" textAnchor="middle">
          FIRST LEARNING MODULE COMPLETED
        </text>
        <rect x="315" y="954" width="370" height="58" rx="29" fill="#0e7490" fillOpacity=".35" stroke="#22d3ee" strokeOpacity=".7" />
        <text x="500" y="992" fill="#cffafe" fontFamily="Arial, sans-serif" fontSize="19" fontWeight="700" letterSpacing="4" textAnchor="middle">
          VERIFIED BY DIGITALSHIELD
        </text>
        <circle cx="115" cy="115" r="7" fill="#67e8f9" />
        <circle cx="885" cy="115" r="7" fill="#67e8f9" />
        <circle cx="115" cy="1005" r="7" fill="#67e8f9" />
        <circle cx="885" cy="1005" r="7" fill="#67e8f9" />
      </svg>

      <button
        type="button"
        onClick={downloadBadge}
        disabled={isDownloading}
        className="mx-auto inline-flex items-center justify-center gap-2 rounded-full bg-[#0e7490] px-4 py-2 text-xs font-semibold text-white transition-colors hover:bg-[#155e75] disabled:cursor-wait disabled:opacity-70"
      >
        <Download className="h-3.5 w-3.5" />
        {isDownloading ? 'Preparing PNG...' : 'Download badge PNG'}
      </button>
      {downloadError && (
        <p role="alert" className="text-center text-xs text-red-700">
          {downloadError}
        </p>
      )}
    </div>
  );
};
