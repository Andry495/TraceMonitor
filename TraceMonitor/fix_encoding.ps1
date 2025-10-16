# PowerShell script to fix encoding issues in Form1.cs
$content = Get-Content "Form1.cs" -Raw -Encoding UTF8

# Replace the problematic line with proper encoding fix
$oldLine = 'if (result == "ॢ襭 ࢠ   .") result = "  ";'
$newLines = @"
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

$content = $content -replace [regex]::Escape($oldLine), $newLines

# Write back to file
Set-Content "Form1.cs" -Value $content -Encoding UTF8

Write-Host "Encoding fix applied successfully!"
