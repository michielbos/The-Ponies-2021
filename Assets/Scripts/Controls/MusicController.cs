using UnityEngine;

public class MusicController : MonoBehaviour {
    public AudioClip[] neighbourhoodSongs;
    public AudioClip[] capSongs;
    public AudioClip[] buyModeSongs;
    public AudioClip[] buildModeSongs;
    public AudioClip[] communityBuildSongs;
    public AudioSource audioSource;
    private MusicType currentMusicType = MusicType.NoMusic;

    private void Update () {
        if (audioSource.clip != null && !audioSource.isPlaying) {
            PlayRandomSong();
        }
    }
    
    public void SwitchMusic (MusicType musicType) {
        currentMusicType = musicType;
        PlayRandomSong();
    }

    private void PlayRandomSong () {
        AudioClip[] songs = GetSongsForType(currentMusicType);
        if (songs.Length <= 0) {
            audioSource.Stop();
            audioSource.clip = null;
            return;
        }
        audioSource.clip = songs[Random.Range(0, songs.Length)];
        audioSource.PlayDelayed(2);
    }

    private AudioClip[] GetSongsForType (MusicType musicType) {
        switch (musicType) {
                case MusicType.NoMusic:
                    return new AudioClip[0];
                case MusicType.Neighbourhood:
                    return neighbourhoodSongs;
                case MusicType.CreateAPony:
                    return capSongs;
                case MusicType.BuyMode:
                    return buyModeSongs;
                case MusicType.BuildMode:
                    return buildModeSongs;
                case MusicType.CommunityBuildMode:
                    return communityBuildSongs;
                default:
                    return new AudioClip[0];
        }
    }
}
