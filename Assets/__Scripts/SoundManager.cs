using UnityEngine;

public class SoundManager : Loader<SoundManager>
{
    [Header("Set in Inspector")]
    public AudioClip arrow;
    public AudioClip fireball;
    public AudioClip rock;

    public AudioClip newGame;
    public AudioClip gameOver;

    public AudioClip level;
    public AudioClip towerBuilt;
    public AudioClip death;
    public AudioClip hit;
}
