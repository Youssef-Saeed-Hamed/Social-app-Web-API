from gtts import gTTS
import os

# Convert text to speech
text = "يا حمار يا ابن الحمار"
tts = gTTS(text=text, lang='ar')

# Save the audio file
tts.save("output1.mp3")

# Play the generated speech
os.system("start output1.mp3")  # Windows
# os.system("mpg321 output.mp3")  # Linux
