from rev_ai import apiclient
import sounddevice as sd # python -m pip install sounddevice --user
from scipy.io.wavfile import write # pip install scipy
from pydub import AudioSegment # convert WAV to mp3

rev_ai_token = "02g01w-kZhbjrtz5qXsiT71gchuHv7SyFFqnRTOAjYYTrJlxV7KA9DkYehOZofxVvOZl_qfQD10LlsiMsu6G--WT7ai2I"
client = apiclient.RevAiAPIClient(rev_ai_token)

SentithinkAPI = "[[AZURE WEB API ENDPOINT]]"
Sentithink_auth = "code=[[AUTH TOKEN]]"

x = float(input("Enter microphone X location: "))
y = float(input("Enter microphone Y location: "))
name = input("Enter microphone name: ")

# Capture the sound snippet and save it to file here
frameRate = 44100  # Sample rate
seconds = 10  # Duration of recording

myrecording = sd.rec(int(seconds * frameRate), samplerate=frameRate, channels=2)
sd.wait()  # Wait until recording is finished
write('voiceRecord.wav', frameRate, myrecording)  # Save as WAV file
convertedAudio = AudioSegment.from_wav('voiceRecord.wav')

convertedAudio.export('voiceRecord.mp3', format='mp3')

rev_ai_callback_url = SentithinkAPI + Sentithink_auth + "&X=" + str(x) + "&Y=" + str(y) + "&name=" + name
job = client.submit_job_local_file('C:\\Users\\a7089\\Desktop\\Programming\\hackMIT\\voiceRecord.wav',callback_url=rev_ai_callback_url)
