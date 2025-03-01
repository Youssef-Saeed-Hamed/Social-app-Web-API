import whisper
from pydub import AudioSegment
import os
import noisereduce as nr
import numpy as np
import librosa
import soundfile as sf

def convert_weba_to_mp3(weba_path):
    """تحويل ملف WEBA إلى MP3"""
    mp3_path = weba_path.replace(".weba", ".mp3")
    audio = AudioSegment.from_file(weba_path, format="webm")  # استخدام webm بدلاً من weba
    audio.export(mp3_path, format="mp3", bitrate="192k")  # تحسين الجودة
    return mp3_path

def preprocess_audio(mp3_path):
    """تحسين جودة الصوت عبر تحويله إلى WAV أحادي بمعدل 16kHz"""
    wav_path = mp3_path.replace(".mp3", ".wav")
    audio = AudioSegment.from_mp3(mp3_path)

    # تحويل الصوت إلى أحادي وتحديد معدل العينة 16kHz
    audio = audio.set_channels(1).set_frame_rate(16000)
    
    # حفظ الملف بعد المعالجة
    audio.export(wav_path, format="wav")
    return wav_path

def denoise_audio(wav_path):
    """إزالة الضوضاء من ملف الصوت وتحسين جودته"""
    y, sr = librosa.load(wav_path, sr=16000)

    # تحسين عملية إزالة الضوضاء
    reduced_noise = nr.reduce_noise(y=y, sr=sr, prop_decrease=0.8, stationary=True)
    
    # حفظ الملف بعد إزالة الضوضاء
    sf.write(wav_path, reduced_noise, sr)

def mp3_to_text(mp3_path):
    """تحليل الصوت وتحويله إلى نص باستخدام Whisper"""
    wav_path = preprocess_audio(mp3_path)  # تحسين جودة الصوت
    denoise_audio(wav_path)  # إزالة الضوضاء
    
    # تحميل نموذج Whisper (يفضل large لأفضل دقة)
    model = whisper.load_model("large")  # يمكن تجربة "medium" إذا كان الجهاز ضعيفًا

    # تحويل الصوت إلى نص
    result = model.transcribe(wav_path, language="ar", temperature=0, word_timestamps=True)
    
    # حذف ملف WAV المؤقت لتوفير المساحة
    os.remove(wav_path)

    return result["text"]

# ✅ **مثال للاستخدام**
weba_file = "1.weba"  
mp3_file = convert_weba_to_mp3(weba_file)  # تحويل WEBA إلى MP3  
text = mp3_to_text(mp3_file)  # استخراج النص من الصوت  
print("Transcribed Text:", text)
