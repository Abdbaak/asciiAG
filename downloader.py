from os import rename
import requests
from imghdr import what
import sys

res = requests.get(sys.argv[1])
outputFile = open("image", "wb")
outputFile.write(res.content)
outputFile.close()
ext = what("image")
rename("image", f"image.{ext}")
print(f"image.{ext}", end="")
