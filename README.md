# SXSEXPHelper
SXSEXPHelper is a very simple wrapper to automate building SFCFix fix batches which involve decompressing payload files from WinSxS. This is not an automated repair tool
and should not be used by those who do not understand the internal mechanisms of Windows Update.

Instructions:

You will need to ensure that SXSEXP has been named to sxsexp64; this is the default name when you download the .exe from the project page. The files you wish to decompress should remain in their corresponding directories and be in the same directory as SXSEXPHelper.exe along with sxsexp64.exe. The application simply needs to executed and it will do the rest for you.

SXSEXP: https://github.com/hfiref0x/SXSEXP
