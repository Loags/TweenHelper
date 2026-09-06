"""Assemble actual Unity Recorder clips with captions; requires ffmpeg with libass."""
import argparse
import pathlib
import re
import subprocess


def timestamp(seconds):
    millis = round(seconds * 1000)
    hours, millis = divmod(millis, 3600000)
    minutes, millis = divmod(millis, 60000)
    seconds, millis = divmod(millis, 1000)
    return f"{hours:02}:{minutes:02}:{seconds:02},{millis:03}"


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("ffmpeg", type=pathlib.Path)
    parser.add_argument("clips", type=pathlib.Path)
    parser.add_argument("output", type=pathlib.Path)
    args = parser.parse_args()
    encoder = str(args.ffmpeg.resolve())
    output = args.output.resolve()
    output.mkdir(parents=True, exist_ok=True)
    subjects = [
        ("presets", "300 reusable presets. Preview, replay, and copy the C# call."),
        ("ui-recipes", "Compose UI feedback with the semantic builder API."),
        ("collections", "Animate collections with reusable layout and ordering options."),
        ("destination-motion", "Guide rewards and UI elements to a precise destination."),
        ("gameplay-feedback", "Add brief feedback to meaningful gameplay interactions."),
        ("ui-sequences", "Coordinate authored UI participants in a single sequence."),
        ("text-and-values", "Animate TMP characters and values with explicit ownership."),
        ("camera-feedback", "Layer camera feedback with interruption and restoration."),
    ]
    playlist, captions, elapsed = [], [], 0.0
    for index, (slug, caption) in enumerate(subjects, 1):
        clip = (args.clips / f"{slug}-1920x1080.mp4").resolve()
        if not clip.is_file() or not clip.stat().st_size:
            raise RuntimeError(f"Missing Recorder clip: {clip}")
        result = subprocess.run([encoder, "-hide_banner", "-i", str(clip)], capture_output=True, text=True)
        duration = re.search(r"Duration: (\d+):(\d+):(\d+\.\d+)", result.stderr)
        if not duration:
            raise RuntimeError(f"Cannot inspect video: {clip}")
        hours, minutes, seconds = map(float, duration.groups())
        length = hours * 3600 + minutes * 60 + seconds
        playlist.append("file '" + clip.as_posix().replace("'", "'\\''") + "'")
        captions.append(f"{index}\n{timestamp(elapsed)} --> {timestamp(elapsed + length)}\n{caption}\n")
        elapsed += length
    (output / "Clips.txt").write_text("\n".join(playlist), encoding="utf-8")
    (output / "TweenHelper-Reel.srt").write_text("\n".join(captions), encoding="utf-8")
    filters = "scale=1760:990,pad=1920:1080:80:0:color=0x0c0c18,subtitles=TweenHelper-Reel.srt:force_style='FontName=Segoe UI,Fontsize=22,Alignment=2,MarginV=10,Outline=1'"
    subprocess.run([encoder, "-y", "-hide_banner", "-loglevel", "warning", "-f", "concat", "-safe", "0", "-i", "Clips.txt", "-vf", filters, "-an", "-c:v", "libx264", "-threads", "2", "-preset", "medium", "-crf", "18", "-pix_fmt", "yuv420p", "-movflags", "+faststart", "TweenHelper-1.3.0-rc.1-Reel.mp4"], cwd=output, check=True)
    print(f"Created {elapsed:.2f}s captioned reel in {output}")


if __name__ == "__main__":
    main()
