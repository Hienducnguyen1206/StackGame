using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class AudioVisualizer : MonoBehaviour
{
    public AudioSource audioSource;
    public Transform[] bars;
    public TextMeshProUGUI musicname;

    public List<AudioClip> musicList;
    public Slider VolumeSlider;
    private int musicIndex ; 

    private float[] spectrumData = new float[64];
    private bool musicPlaying = true;

    private void Start()
    {   
        RandomMusic();
        VolumeSlider.value = 1;
        float centerOffset = (bars.Length - 1) / 2f;
        float spacing = 0.5f;

        for (int i = 0; i < bars.Length; i++)
        {
            float xPos = (i - centerOffset) * spacing;
            bars[i].position = new Vector3(xPos, 0, 0);
        }
    }

    private void Update()
    {  
        audioSource.volume = VolumeSlider.value;
        musicname.text = audioSource.clip.name;
        audioSource.GetSpectrumData(spectrumData, 0, FFTWindow.BlackmanHarris);
        int spectrumLength = spectrumData.Length / 2;
        for (int i = 0; i < bars.Length / 2; i++)
        {
            float frequencyDataLeft = spectrumData[i];
            float frequencyDataRight = spectrumData[spectrumLength - i - 1];

            bars[i].localScale = new Vector3(1, Mathf.Clamp(frequencyDataLeft * 10, 0.3f, 5f), 1);
            bars[bars.Length - i - 1].localScale = new Vector3(1, Mathf.Clamp(frequencyDataRight * 10, 0.3f, 5f), 1);
        }

        if ( audioSource != null)
        {
          if(!audioSource.isPlaying && musicPlaying)
            {   
                NextMusic();
            }
           
        }
    }

    public void NextMusic()
    {
        if (musicIndex + 1 < musicList.Count)
        {   
            audioSource.clip = musicList[musicIndex + 1];
            audioSource.Play();
            musicIndex++;
        }
        else
        {
            audioSource.clip = musicList[0];
            audioSource.Play();
            musicIndex = 0;
        }
    }

    public void PreviousMusic() {
        if (musicIndex - 1 >= 0 )
        {
            audioSource.clip = musicList[musicIndex-1];
            audioSource.Play();
            musicIndex--;
        }
        else
        {
            audioSource.clip = musicList[musicList.Count-1];
            audioSource.Play();
            musicIndex = musicList.Count -1;
        }
    }

    public void PlayMusic() {
        if (musicPlaying == false)
        {   
           
            audioSource.Play();
            musicPlaying = true;
        }
        else
        {
            
            audioSource.Pause();
            musicPlaying = false;
        }
    }

   

    public void RandomMusic()
    {
        musicIndex = Random.Range(0, musicList.Count);
        audioSource.clip = musicList[musicIndex];
        audioSource.Play();
    }

    public void LoopMusic()
    {
        audioSource.loop = true;
    }

}
