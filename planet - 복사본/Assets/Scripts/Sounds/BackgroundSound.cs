
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundSound : MonoBehaviour
{
    public Transform target1;
    public Transform target2;

    public AudioClip[] musics;
    public float[] deltaDistances;

    private int currentHead;
    private AudioSource[] audioSources;
    private float distance;

    private void Start()
    {
        audioSources = new AudioSource[musics.Length];
        distance = Vector3.Distance(target1.position, target2.position);
        for(int i=0;i<musics.Length;i++)
        {
            gameObject.AddComponent<AudioSource>();
        }
        audioSources = GetComponents<AudioSource>();
    }

    private void Update()
    {
        distance = Vector3.Distance(target1.position, target2.position);
        for(int i=0;i<musics.Length;i++)
        {
            if(distance < deltaDistances[i])
            {
                PlayMusic(i + 1);
            }
        }
    }

    public void PlayMusic(int num)
    {
        if(currentHead != num)
        {
            if(currentHead > num)
            {
                RemoveMusic();
            } 
            else if(currentHead < num)
            {
                AddMusic();
            }
        }
    }

    public void AddMusic()
    {
        if(currentHead < musics.Length)
        {
            audioSources[currentHead].clip = musics[currentHead];
            audioSources[currentHead].loop = true;
            audioSources[currentHead].Play();
            if(currentHead != 0)
            {
                audioSources[currentHead].time = audioSources[currentHead - 1].time;
            }

            currentHead++;
        }
    }

    public void RemoveMusic()
    {
        if(currentHead != 0)
        {
            audioSources[currentHead - 1].Stop();
            currentHead--;
        }
    }
}
