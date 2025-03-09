using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance;

    [SerializeField] private AudioSource aSource;
    [SerializeField] private List<AudioClip> clips;
    [SerializeField] private HashMap<string, AudioClip> audioMap;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        aSource = GetComponent<AudioSource>();

        audioMap = new HashMap<string, AudioClip>();
        foreach (AudioClip clip in clips)
        {
            audioMap.Put(clip.name, clip);
        }

    }

    public void PlayAudio(string name)
    {
        if (audioMap.ContainsKey(name))
        {
            //aSource.clip = audioMap.Get(name);
            aSource.PlayOneShot(audioMap.Get(name));
            //Debug.Log("Play");
        }
        else
        {
            //Debug.Log("Sound " + name + " not found");
        }

        void PlayAudio(string name, float pitch)
        {
            if (audioMap.ContainsKey(name))
            {
                //aSource.clip = audioMap.Get(name);
                aSource.pitch = pitch;
                aSource.PlayOneShot(audioMap.Get(name));
                //Debug.Log("Play");
            }
            else
            {
                //Debug.Log("Sound " + name + " not found");
            }
        }
    }
}