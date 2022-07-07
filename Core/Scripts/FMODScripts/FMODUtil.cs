using System.Collections;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using Serilog;
using UnityEngine;
using UnityEngine.Events;

public class FMODUtil //: MonoBehaviour
{
    public static void PlayFeedback(GameObject sourceGameObject, string feedbackEventName, string parameterName = "Pan",
        float value = 0)
    {
        if (string.IsNullOrEmpty(feedbackEventName)) return;

        // one shot with parameters
        var instance = RuntimeManager.CreateInstance(feedbackEventName);
        instance.setParameterByName(parameterName, value);
        instance.set3DAttributes(RuntimeUtils.To3DAttributes(sourceGameObject));
        instance.start();
        instance.release();
    }

    public static IEnumerator PlayStream(string url, UnityAction<bool> isComplete, GameObject objectGameObject)
    {
        //IntPtr parameterPtr = new IntPtr();
        //var parameter = (FMOD.Studio.PROGRAMMER_SOUND_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(FMOD.Studio.PROGRAMMER_SOUND_PROPERTIES));

        var sound = new Sound();
        //sound.handle = parameter.sound;

        // load the sound
        RESULT result = RuntimeManager.CoreSystem.createStream(url, MODE.NONBLOCKING | MODE.LOOP_NORMAL | MODE._3D, out sound);
        if (result != RESULT.OK) ReportResult(result, "CoreSystem.createStream");
        
        sound.set3DMinMaxDistance(0.1f, 1);
        RuntimeManager.CoreSystem.getMasterChannelGroup(out ChannelGroup channelgroup);

 

        // wait until sound is fully loaded
        OPENSTATE state;
        do
        {
            sound.getOpenState(out state, out uint _, out bool _, out _);
            yield return new WaitForSeconds(0.03f);
        } while (state != OPENSTATE.READY);


        //attempting to attach the sound to game object
        EventInstance instance = new EventInstance(sound.handle);
        RuntimeManager.AttachInstanceToGameObject(instance, objectGameObject.transform, objectGameObject.GetComponent<Rigidbody>());

        // play the sound

        VECTOR pos;
        pos.x = objectGameObject.transform.position.x;
        pos.y = objectGameObject.transform.position.y;
        pos.z = objectGameObject.transform.position.z;

        VECTOR vel;
        vel.x = 0;
        vel.y = 0;
        vel.z = 1;

        result = RuntimeManager.CoreSystem.playSound(sound, channelgroup, false, out var channel);
        ReportResult(result, "RuntimeManager.CoreSystem.playSound");
        channel.set3DAttributes(ref pos, ref vel);
        channel.set3DMinMaxDistance(0.1f, 1);
        bool isPlaying;
        do
        {
            channel.isPlaying(out isPlaying);
            yield return new WaitForSeconds(0.03f);
        } while (isPlaying);

        isComplete?.Invoke(true);
        channel.clearHandle();
        yield return null;

    }

    public static void ReportResult(RESULT result, string msg)
    {
        Log.Debug($"{result} {msg}");
    }

    public static RESULT Nonblockcallback(Sound sound, RESULT result)
    {
        Log.Debug($"Sound loaded! {result}");
        return RESULT.OK;
    }
}