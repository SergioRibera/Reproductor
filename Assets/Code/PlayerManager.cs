using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Random = UnityEngine.Random;

public class PlayerManager : MonoBehaviour
{
    public bool SaveData;
    public int tiempoDeEsperaEntreCancion = 0;
    public Slider volumeConfig;
    public Slider timerShow;
    public GameObject prefab;
    public Transform containerListMusics;
    public List<AudioClip> audios = new List<AudioClip>();

    AudioSource player;
    [SerializeField] int index = 0;
    [SerializeField] List<AudioClip> musicasRandoms = new List<AudioClip>();

    void Start()
    {
        player = GetComponent<AudioSource>();
        if (SaveData)
        {
            if (PlayerPrefs.HasKey("Volumen"))
            {
                volumeConfig.value = PlayerPrefs.GetFloat("Volumen");
                player.volume = PlayerPrefs.GetFloat("Volumen");
            }
        }
        musicasRandoms = audios;
        UpdatingListVisual();
    }
    public void UpdatingListVisual()
    {
        if (containerListMusics.childCount > 0)
        {
            for (int i = 0; i < containerListMusics.childCount; i++)
            {
                Destroy(containerListMusics.GetChild(i).gameObject);
            }
        }
        for (int i = 0; i < musicasRandoms.Count; i++)
        {
            GameObject go = Instantiate(prefab, containerListMusics);
            go.GetComponent<Button>().onClick.AddListener(() => {
                index = i;
                player.clip = musicasRandoms[index];
                player.Play();
                StartCoroutine(UpdateTimerBar());
            });
            go.GetComponentInChildren<Text>().text = musicasRandoms[i].name;
        }
    }
    public void ShowVolumeBar()
    {
        volumeConfig.gameObject.SetActive(!volumeConfig.gameObject.activeSelf);
    }
    public void ChangeVolume(float v)
    {
        player.volume = v;
        if (SaveData)
            PlayerPrefs.SetFloat("Volumen", v);
    }
    public void ChangeTime(float t)
    {
        player.time = t;
    }

    [SerializeField] bool Pause;
    public void Play()
    {
        if (index <= 0) index = 0;
        if (index > musicasRandoms.Count - 1) index = 0;
        if (!player.isPlaying)
        {
            StartCoroutine(NextSong());
        }
        print("Play " + musicasRandoms[index].name);
        StartCoroutine(UpdateTimerBar());
        //player.UnPause();
    }
    public void Pausa()
    {
        if (player.isPlaying)
            player.Pause();
        StopAllCoroutines();
        Pause = true;
        print("Pause " + musicasRandoms[index].name);
    }
    public void Stop()
    {
        if (player.isPlaying)
            player.Stop();
    }
    public void Prev()
    {
        index--; 
        if (index < 0) index = musicasRandoms.Count - 1;
        StopAllCoroutines();
        StartCoroutine(NextSong(true));
        print("Preview " + musicasRandoms[index].name);
    }
    public void Next()
    {
        index++;
        if (index > (musicasRandoms.Count - 1)) index = 0;
        StopAllCoroutines();
        StartCoroutine(NextSong(true));
        print("Next " + musicasRandoms[index].name);
    }

    public void DisminuirVelocidad()
    {
        Time.timeScale = 1;
    }
    public void AumentarVelocidad()
    {
        Time.timeScale = 4;
    }

    public void RandomMusic()
    {
        StartCoroutine(NewRandomList());
        Stop();
        Play();
    }

    IEnumerator NextSong(bool directNext = false)
    {
        while (player.isPlaying && !directNext)
        {
            yield return null;
        }
        StartCoroutine(Wait());
        player.clip = musicasRandoms[index];
        player.Play();
        StartCoroutine(UpdateTimerBar());
        print("Playing " + musicasRandoms[index].name);
    }
    IEnumerator Wait()
    {
        yield return new WaitForSecondsRealtime(tiempoDeEsperaEntreCancion);
        print("Espera terminada");
    }
    IEnumerator NewRandomList()
    {
        List<AudioClip> a = new List<AudioClip>();
        List<int> indexs = new List<int>();
        while(a.Count < audios.Count)
        {
            int i = Random.Range(0, a.Count - 1);
            if (!indexs.Contains(i))
            {
                indexs.Add(i);
                a.Add(audios[i]);
            }
            yield return null;
        }
        musicasRandoms = a;
    }
    
    IEnumerator UpdateTimerBar()
    {
        while (player.isPlaying)
        {
            timerShow.value = player.time;
            timerShow.maxValue = musicasRandoms[index].length;
            yield return null;
            print("updating time");
        }
    }
    
    bool CheckNullSong
    {
        get
        {
            return player.clip == null;
        }
    }
}