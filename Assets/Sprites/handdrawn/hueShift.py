from PIL import Image
import sys

# open a sprite sheet and create various hue-shifted versions from it
print "Hue shifted image generator.  Ed Paradis"

if len(sys.argv) < 2:
  print "Usage: %s [filename]" % sys.argv[0]
  exit(-1)

# the original sprite sheet
origFilename = sys.argv[1]  #'/Users/ed/Desktop/nonfree game assets/FF6 frog.png'
origSheet = Image.open( origFilename)

# from http://stackoverflow.com/questions/7274221/changing-image-hue-with-python-pil?rq=1
#import Image
import numpy as np
import colorsys

rgb_to_hsv = np.vectorize(colorsys.rgb_to_hsv)
hsv_to_rgb = np.vectorize(colorsys.hsv_to_rgb)

def shift_hue(arr, hout):
    r, g, b, a = np.rollaxis(arr, axis=-1)
    h, s, v = rgb_to_hsv(r, g, b)
    h = hout
    r, g, b = hsv_to_rgb(h, s, v)
    arr = np.dstack((r, g, b, a))
    return arr

def colorize(image, hue):
    """
    Colorize PIL image `original` with the given
    `hue` (hue within 0-360); returns another PIL image.
    """
    img = image.convert('RGBA')
    arr = np.array(np.asarray(img).astype('float'))
    new_img = Image.fromarray(shift_hue(arr, hue/360.).astype('uint8'), 'RGBA')

    return new_img

print "Original file: %s" % origFilename

for shiftAmt in range(0,360,60):
  shifted = colorize( origSheet, shiftAmt)
  shiftedFilename = 'shifted%03d.png' % shiftAmt
  print "Creating file: %s" % shiftedFilename
  shifted.save( shiftedFilename , 'PNG')





