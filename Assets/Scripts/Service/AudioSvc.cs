/****************************************************
    文件：AudioSvc.cs
	作者：AnderTroy
    邮箱: 1329524041@qq.com
    日期：2019/5/7 11:27:30
    功能：声音播放
*****************************************************/
using UnityEngine;
public class AudioSvc : MonoBehaviour
{
    public static AudioSvc Instance = null;
    public AudioSource BgAudio;//播放背景音乐
    public AudioSource UiAudio;//播放按键音效
    public void InitSvc()
    {
        Instance = this;
        Debug.Log(Instance.GetType());
    }
    public void PlayBgAudioMusic(string audioName,bool isLoop=true)
    {
        AudioClip audioMusic = ResSvc.Instance.LoadAudio("ResAudio/" + audioName, true);//保存音频地址
        if (BgAudio.clip==null||BgAudio.clip.name!=audioMusic.name)//判断音频是否存在，替换成背景音乐
        {
            BgAudio.clip = audioMusic;
            BgAudio.loop = isLoop;
            BgAudio.Play();
        }
    }
    public void PlayUiAudioMusic(string audioName)
    {
        AudioClip audioMusicClip = ResSvc.Instance.LoadAudio("ResAudio/" + audioName, true);
        UiAudio.clip = audioMusicClip;
        UiAudio.Play();
    }

    public void PlayCharAudio(string charName, AudioSource source)
    {
        AudioClip audioPlayerClip = ResSvc.Instance.LoadAudio("ResAudio/" + charName);
        source.clip = audioPlayerClip;
        source.Play();
    }

    public void StopBgMusic()
    {
        if (BgAudio!=null)
        {
            BgAudio.Stop();
        }
    }
}