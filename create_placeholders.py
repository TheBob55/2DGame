#!/usr/bin/env python3
"""
Create placeholder PNG images for the Godot game
"""
import struct
import zlib

def create_png(filename, width, height, r, g, b, a=255):
    """Create a simple solid color PNG file"""

    # PNG signature
    png_signature = b'\x89PNG\r\n\x1a\n'

    # IHDR chunk
    ihdr_data = struct.pack('>IIBBBBB', width, height, 8, 6, 0, 0, 0)
    ihdr_chunk = create_chunk(b'IHDR', ihdr_data)

    # IDAT chunk (image data)
    raw_data = b''
    for y in range(height):
        raw_data += b'\x00'  # Filter type 0 (None)
        for x in range(width):
            raw_data += bytes([r, g, b, a])

    compressed_data = zlib.compress(raw_data, 9)
    idat_chunk = create_chunk(b'IDAT', compressed_data)

    # IEND chunk
    iend_chunk = create_chunk(b'IEND', b'')

    # Write PNG file
    with open(filename, 'wb') as f:
        f.write(png_signature)
        f.write(ihdr_chunk)
        f.write(idat_chunk)
        f.write(iend_chunk)

def create_chunk(chunk_type, data):
    """Create a PNG chunk"""
    length = struct.pack('>I', len(data))
    crc = zlib.crc32(chunk_type + data) & 0xffffffff
    crc_bytes = struct.pack('>I', crc)
    return length + chunk_type + data + crc_bytes

# Create player sprites (blue color)
print("Creating player sprites...")
create_png('art/playerGrey_up1.png', 100, 100, 100, 100, 200, 255)
create_png('art/playerGrey_up2.png', 100, 100, 110, 110, 210, 255)
create_png('art/playerGrey_walk1.png', 100, 100, 100, 100, 200, 255)
create_png('art/playerGrey_walk2.png', 100, 100, 110, 110, 210, 255)

# Create enemy sprites (red color - flying)
print("Creating enemy sprites...")
create_png('art/enemyFlyingAlt_1.png', 100, 100, 200, 100, 100, 255)
create_png('art/enemyFlyingAlt_2.png', 100, 100, 210, 110, 110, 255)

# Create enemy sprites (green color - swimming)
create_png('art/enemySwimming_1.png', 100, 100, 100, 200, 100, 255)
create_png('art/enemySwimming_2.png', 100, 100, 110, 210, 110, 255)

# Create enemy sprites (orange color - walking)
create_png('art/enemyWalking_1.png', 100, 100, 200, 150, 100, 255)
create_png('art/enemyWalking_2.png', 100, 100, 210, 160, 110, 255)

print("Placeholder images created successfully!")
print("\nNote: These are simple colored squares.")
print("Download proper game assets from the Godot tutorial for the full experience.")
