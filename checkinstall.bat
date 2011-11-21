ECHO Checking for csc.exe compiler.

IF EXIST C:\windows\microsoft.net\Framework\v2.0.5072blah7\csc.exe (
	ECHO SUCCESS: csc.exe file found.
) ELSE (
	ECHO ERROR: csc.exe file not found, opening Internet Explorer.
	start iexplore "http://www.microsoft.com/download/en/details.aspx?displaylang=en&id=19"
)

PAUSE