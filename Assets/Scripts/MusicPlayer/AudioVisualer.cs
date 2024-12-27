using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class AudioVisualizer : MonoBehaviour
{
    public AudioSource audioSource;
    public Transform[] bars;
    public TextMeshProUGUI musicname;

    public List<AudioClip> musicList;
    public Slider VolumeSlider;

    private int musicIndex;
    private float[] outputData = new float[256];
    private bool musicPlaying = true;

    private void Start()
    {
        if (audioSource == null || musicList == null || musicList.Count == 0)
        {
            Debug.LogError("AudioSource hoặc danh sách nhạc chưa được thiết lập.");
            return;
        }

        RandomMusic();
        VolumeSlider.value = audioSource.volume;

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
        if (bars == null || bars.Length == 0 || outputData.Length < bars.Length)
            return;

        audioSource.volume = VolumeSlider.value;

        if (musicname.text != audioSource.clip.name)
        {
            musicname.text = audioSource.clip.name;
        }

        // Sử dụng GetOutputData thay vì GetSpectrumData
        audioSource.GetOutputData(outputData, 0);

        for (int i = 0; i < bars.Length; i++)
        {
            float frequencyData = outputData[i] * 10; // Áp dụng một hệ số để tăng cường độ cao
            bars[i].localScale = new Vector3(1, Mathf.Clamp(frequencyData, 0.3f, 5f), 1);
        }

        if (!audioSource.isPlaying && musicPlaying)
        {
            NextMusic();
        }
    }

    public void NextMusic()
    {
        musicIndex = (musicIndex + 1) % musicList.Count;
        audioSource.clip = musicList[musicIndex];
        audioSource.Play();
    }

    public void PreviousMusic()
    {
        musicIndex = (musicIndex - 1 + musicList.Count) % musicList.Count;
        audioSource.clip = musicList[musicIndex];
        audioSource.Play();
    }

    public void PlayMusic()
    {
        if (musicPlaying)
        {
            audioSource.Pause();
        }
        else
        {
            audioSource.Play();
        }
        musicPlaying = !musicPlaying;
    }

    public void RandomMusic()
    {
        int previousIndex = musicIndex;
        do
        {
            musicIndex = Random.Range(0, musicList.Count);
        } while (musicIndex == previousIndex);

        audioSource.clip = musicList[musicIndex];
        audioSource.Play();
    }

    public void LoopMusic()
    {
        audioSource.loop = !audioSource.loop;
    }
}
