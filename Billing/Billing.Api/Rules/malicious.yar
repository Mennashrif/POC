rule DetectMaliciousContent
{
    meta:
        description = "Detects malicious patterns in uploaded files"
        author = "eSAP POC"

    strings:
        // Script injection
        $script     = "<script"       ascii nocase
        $eval       = "eval("         ascii nocase

        // Shell commands
        $powershell = "powershell"    ascii nocase
        $cmd        = "cmd.exe"       ascii nocase

        // SQL injection
        $drop       = "DROP TABLE"    ascii nocase
        $select     = "SELECT *"      ascii nocase

        // PHP shell (common in image metadata attacks)
        $php        = "<?php"         ascii nocase

    condition:
        any of them
}
