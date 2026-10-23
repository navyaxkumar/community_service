using System.Text.Json;
using System.Text.RegularExpressions;
using DigitalShield.API.Constants;

namespace DigitalShield.API.Helpers;

public sealed record ContentMetadata(
    string TargetAudience,
    string DifficultyLevel,
    int EstimatedMinutes,
    string? LearningObjectives,
    string? SafetyTips);

public static class ContentMetadataParser
{
    private static readonly Regex MetadataCommentRegex = new(
        @"<!--\s*digitalshield-metadata:\s*(\{.*?\})\s*-->",
        RegexOptions.Compiled | RegexOptions.Singleline);

    private static readonly Regex FrontmatterRegex = new(
        @"^---\r?\n(.*?)\r?\n---\r?\n?",
        RegexOptions.Compiled | RegexOptions.Singleline);

    private static readonly Regex DescriptionTagRegex = new(
        @"^\[Audience:\s*([^|\]]+)(?:\|\s*Difficulty:\s*([^|\]]+))?(?:\|\s*Time:\s*(\d+)\s*(?:mins|min)?)?\]\s*",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public static ContentMetadata Parse(string? content, string? description = null, string? title = null)
    {
        var rawContent = content ?? string.Empty;

        // 1. Try parsing JSON metadata comment
        var commentMatch = MetadataCommentRegex.Match(rawContent);
        if (commentMatch.Success)
        {
            try
            {
                var json = commentMatch.Groups[1].Value;
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                var audience = root.TryGetProperty("audience", out var audProp)
                    ? audProp.GetString()
                    : null;
                var difficulty = root.TryGetProperty("difficulty", out var diffProp)
                    ? diffProp.GetString()
                    : "Beginner";
                var duration = root.TryGetProperty("duration", out var durProp) && durProp.TryGetInt32(out var dVal)
                    ? dVal
                    : 5;
                var objectives = root.TryGetProperty("objectives", out var objProp)
                    ? objProp.GetString()
                    : null;
                var safetyTips = root.TryGetProperty("safetyTips", out var tipProp)
                    ? tipProp.GetString()
                    : null;

                return new ContentMetadata(
                    AudienceConstants.Normalize(audience),
                    NormalizeDifficulty(difficulty),
                    Math.Max(1, duration),
                    objectives,
                    safetyTips);
            }
            catch
            {
                // Fall back to secondary strategies if JSON parsing fails
            }
        }

        // 2. Try parsing YAML frontmatter
        var frontmatterMatch = FrontmatterRegex.Match(rawContent);
        if (frontmatterMatch.Success)
        {
            var yamlBlock = frontmatterMatch.Groups[1].Value;
            var parsed = ParseSimpleYaml(yamlBlock);
            if (parsed != null)
            {
                return parsed;
            }
        }

        // 3. Try parsing description bracket tags
        if (!string.IsNullOrWhiteSpace(description))
        {
            var tagMatch = DescriptionTagRegex.Match(description);
            if (tagMatch.Success)
            {
                var aud = tagMatch.Groups[1].Value.Trim();
                var diff = tagMatch.Groups[2].Success ? tagMatch.Groups[2].Value.Trim() : "Beginner";
                var time = tagMatch.Groups[3].Success && int.TryParse(tagMatch.Groups[3].Value, out var t) ? t : 5;

                return new ContentMetadata(
                    AudienceConstants.Normalize(aud),
                    NormalizeDifficulty(diff),
                    Math.Max(1, time),
                    null,
                    null);
            }
        }

        // 4. Heuristic inference based on title, description, or content keywords
        var inferredAudience = InferAudience(title, description, rawContent);
        var inferredDifficulty = InferDifficulty(inferredAudience);
        var inferredDuration = InferDuration(rawContent);

        return new ContentMetadata(
            inferredAudience,
            inferredDifficulty,
            inferredDuration,
            null,
            null);
    }

    public static string StripMetadata(string? content)
    {
        if (string.IsNullOrEmpty(content))
        {
            return string.Empty;
        }

        var stripped = MetadataCommentRegex.Replace(content, string.Empty);
        stripped = FrontmatterRegex.Replace(stripped, string.Empty);
        return stripped.Trim();
    }

    public static string CleanDescription(string? description)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            return string.Empty;
        }

        return DescriptionTagRegex.Replace(description, string.Empty).Trim();
    }

    public static string EmbedMetadata(string rawContent, ContentMetadata metadata)
    {
        var cleanBody = StripMetadata(rawContent);
        var payload = new
        {
            audience = AudienceConstants.Normalize(metadata.TargetAudience),
            difficulty = NormalizeDifficulty(metadata.DifficultyLevel),
            duration = Math.Max(1, metadata.EstimatedMinutes),
            objectives = metadata.LearningObjectives,
            safetyTips = metadata.SafetyTips
        };

        var json = JsonSerializer.Serialize(payload);
        return $"<!--digitalshield-metadata:{json}-->\n\n{cleanBody}";
    }

    public static string NormalizeDifficulty(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return "Beginner";
        }

        var trimmed = value.Trim().ToLowerInvariant();
        return trimmed switch
        {
            "advanced" or "hard" => "Advanced",
            "intermediate" or "medium" => "Intermediate",
            _ => "Beginner"
        };
    }

    private static string InferAudience(string? title, string? description, string content)
    {
        var text = $"{title} {description} {content}".ToLowerInvariant();

        if (text.Contains("early learner") || text.Contains("ages 5") || text.Contains("grown-up") || text.Contains("grown up") || text.Contains("secret word"))
        {
            return AudienceConstants.EarlyLearners;
        }

        if (text.Contains("kid") || text.Contains("children") || text.Contains("roblox") || text.Contains("game coin") || text.Contains("game reward") || text.Contains("ages 8"))
        {
            return AudienceConstants.Children;
        }

        if (text.Contains("teen") || text.Contains("social media") || text.Contains("gaming account trade") || text.Contains("influencer") || text.Contains("collab") || text.Contains("ages 13"))
        {
            return AudienceConstants.Teens;
        }

        if (text.Contains("internship") || text.Contains("recruitment fee") || text.Contains("college") || text.Contains("campus") || text.Contains("work-from-home") || text.Contains("young adult"))
        {
            return AudienceConstants.YoungAdults;
        }

        if (text.Contains("senior") || text.Contains("pension") || text.Contains("elderly") || text.Contains("electricity bill") || text.Contains("first-time digital") || text.Contains("remote control app") || text.Contains("anydesk"))
        {
            return AudienceConstants.Seniors;
        }

        if (text.Contains("business email") || text.Contains("bec") || text.Contains("executive") || text.Contains("mfa fatigue") || text.Contains("wire redirection") || text.Contains("procurement") || text.Contains("workplace"))
        {
            return AudienceConstants.Professionals;
        }

        return AudienceConstants.All;
    }

    private static string InferDifficulty(string audience)
    {
        return audience switch
        {
            AudienceConstants.EarlyLearners => "Beginner",
            AudienceConstants.Children => "Beginner",
            AudienceConstants.Seniors => "Beginner",
            AudienceConstants.Teens => "Intermediate",
            AudienceConstants.YoungAdults => "Intermediate",
            AudienceConstants.Adults => "Intermediate",
            AudienceConstants.Professionals => "Advanced",
            _ => "Beginner"
        };
    }

    private static int InferDuration(string content)
    {
        var words = content.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries).Length;
        if (words <= 100) return 2;
        if (words <= 250) return 3;
        if (words <= 500) return 5;
        return 7;
    }

    private static ContentMetadata? ParseSimpleYaml(string yaml)
    {
        try
        {
            string? audience = null;
            string? difficulty = null;
            int duration = 5;
            string? objectives = null;
            string? safetyTips = null;

            using var reader = new StringReader(yaml);
            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                var colonIndex = line.IndexOf(':');
                if (colonIndex <= 0) continue;

                var key = line[..colonIndex].Trim().ToLowerInvariant();
                var val = line[(colonIndex + 1)..].Trim().Trim('"', '\'');

                switch (key)
                {
                    case "targetaudience" or "audience":
                        audience = val;
                        break;
                    case "difficultylevel" or "difficulty":
                        difficulty = val;
                        break;
                    case "estimatedminutes" or "duration":
                        if (int.TryParse(val, out var d)) duration = d;
                        break;
                    case "learningobjectives" or "objectives":
                        objectives = val;
                        break;
                    case "safetytips" or "tips":
                        safetyTips = val;
                        break;
                }
            }

            return new ContentMetadata(
                AudienceConstants.Normalize(audience),
                NormalizeDifficulty(difficulty),
                Math.Max(1, duration),
                objectives,
                safetyTips);
        }
        catch
        {
            return null;
        }
    }
}
