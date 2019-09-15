from rev_ai import apiclient
import sounddevice as sd # python -m pip install sounddevice --user
from scipy.io.wavfile import write # pip install scipy

def main():
    rev_ai_token = "02g01w-kZhbjrtz5qXsiT71gchuHv7SyFFqnRTOAjYYTrJlxV7KA9DkYehOZofxVvOZl_qfQD10LlsiMsu6G--WT7ai2I"
    client = apiclient.RevAiAPIClient(rev_ai_token)

    SentithinkAPI = "https://sentithinkfunction.azurewebsites.net/api/RecordSnippet?"
    Sentithink_auth = "code=cai3FEcCtUChUkje59jabACIf9N2/Ea0G6UMvj7kS6xX9TBHim0mPg=="

    x = float(input("Enter microphone X location: "))
    y = float(input("Enter microphone Y location: "))
    name = input("Enter microphone name: ")

    # Capture the sound snippet and save it to file here
    frameRate = 44100  # Sample rate
    seconds = 60  # Duration of recording

    loop = 1
    while True:
        print("Starting recording for loop " + str(loop))
        myrecording = sd.rec(int(seconds * frameRate), samplerate=frameRate, channels=2)
        sd.wait()  # Wait until recording is finished
        write('voiceRecord.wav', frameRate, myrecording)  # Save as WAV file

        rev_ai_callback_url = SentithinkAPI + Sentithink_auth + "&X=" + str(x) + "&Y=" + str(y) + "&name=" + name
        job = client.submit_job_local_file('C:\\Users\\a7089\\Desktop\\Programming\\hackMIT\\voiceRecord.wav', callback_url=rev_ai_callback_url)
        print("This is loop " + str(loop))
        loop += 1
main()
