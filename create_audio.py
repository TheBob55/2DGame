#!/usr/bin/env python3
"""
Create placeholder audio files for the Godot game
"""
import wave
import struct
import math

def create_beep_wav(filename, frequency=440, duration=0.5, sample_rate=44100):
    """Create a simple beep sound as a WAV file"""
    num_samples = int(sample_rate * duration)

    with wave.open(filename, 'w') as wav_file:
        # Set parameters: nchannels, sampwidth, framerate, nframes, comptype, compname
        wav_file.setparams((1, 2, sample_rate, num_samples, 'NONE', 'not compressed'))

        # Generate samples with envelope to avoid clicks
        for i in range(num_samples):
            # Exponential decay envelope for death sound
            envelope = math.exp(-5.0 * i / num_samples)

            # Descending frequency for dramatic effect
            freq = frequency * (1.0 - 0.5 * i / num_samples)

            # Generate sine wave
            value = int(32767.0 * envelope * math.sin(2.0 * math.pi * freq * i / sample_rate))
            data = struct.pack('<h', value)
            wav_file.writeframes(data)

def create_music_wav(filename, duration=10.0, sample_rate=44100):
    """Create a simple looping background music as a WAV file"""
    num_samples = int(sample_rate * duration)

    # Simple chord progression frequencies
    notes = [
        (262, 330, 392),  # C major
        (294, 370, 440),  # D minor
        (247, 311, 370),  # B diminished
        (262, 330, 392),  # C major
    ]

    with wave.open(filename, 'w') as wav_file:
        wav_file.setparams((1, 2, sample_rate, num_samples, 'NONE', 'not compressed'))

        beats_per_second = 2  # 120 BPM
        samples_per_beat = sample_rate / beats_per_second

        for i in range(num_samples):
            # Determine which chord we're on
            beat = int(i / samples_per_beat)
            chord_index = (beat // 2) % len(notes)
            chord = notes[chord_index]

            # Mix the three notes of the chord
            value = 0
            for freq in chord:
                value += 0.15 * math.sin(2.0 * math.pi * freq * i / sample_rate)

            # Add a simple melody on top
            melody_freq = chord[0] * 2  # Octave above root
            value += 0.2 * math.sin(2.0 * math.pi * melody_freq * i / sample_rate)

            # Gentle envelope
            t = (i % samples_per_beat) / samples_per_beat
            envelope = 0.5 + 0.5 * math.sin(math.pi * t)
            value *= envelope

            # Convert to 16-bit integer
            int_value = int(32767.0 * value * 0.5)  # 0.5 for overall volume
            int_value = max(-32767, min(32767, int_value))  # Clamp

            data = struct.pack('<h', int_value)
            wav_file.writeframes(data)

print("Creating audio files...")

# Create death sound
print("Creating gameover.wav...")
create_beep_wav('art/gameover.wav', frequency=440, duration=0.8)

# Create background music
print("Creating background music WAV...")
create_music_wav('art/background_music.wav', duration=16.0)

print("\nAudio files created successfully!")
print("\nNote: Created WAV files. For OGG conversion, you'll need ffmpeg:")
print("  ffmpeg -i art/background_music.wav -c:a libvorbis -q:a 4 'art/House In a Forest Loop.ogg'")
