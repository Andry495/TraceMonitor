# Simple PowerShell script to fix encoding issues
$filePath = "Form1.cs"
$content = Get-Content $filePath -Raw

# Find and replace the problematic line
$pattern = 'if \(result == ".*"\) result = ".*";'
$replacement = @"
                        // Fix encoding issues - replace corrupted text with proper message
                        if (result.Contains("Request timed out") || result.Contains("  "))
                        {
                            result = "Request timed out";
                        }
                        else if (result.Contains("Destination host unreachable") || result.Contains("  "))
                        {
                            result = "Destination host unreachable";
                        }
"@

$content = $content -replace $pattern, $replacement

# Write back to file
Set-Content $filePath -Value $content -Encoding UTF8

Write-Host "Encoding fix applied successfully!"
