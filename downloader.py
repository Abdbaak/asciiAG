import requests
import sys
import re


def getExtension(url: str):
    extensionRegex = re.compile(r"\.(bmp|gif|exif|jpg|jpeg|png|tiff)", re.IGNORECASE)
    mo = extensionRegex.search(url)
    ext = mo.group()
    return ext


ext = getExtension(sys.argv[1])
res = requests.get(sys.argv[1])
filename = f"image{ext}"
outputFile = open(filename, "wb")
outputFile.write(res.content)
print(f"image{ext}", end="")
