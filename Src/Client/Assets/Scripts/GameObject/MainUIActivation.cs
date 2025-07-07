using UnityEngine.Playables;
using UnityEngine.Timeline;

public class MainUIActivation : PlayableBehaviour
{
    public bool active;
    public ActivationControlPlayable.PostPlaybackState postPlayback;

    private bool initialActiveState;
    private bool hasProcessed;

    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        if (UIMain.Instance == null) return;

        if (!hasProcessed)
        {
            initialActiveState = UIMain.Instance.Show;
            hasProcessed = true;
        }

        UIMain.Instance.Show = active;

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.MusicMute(true);
        }
    }

    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        if (UIMain.Instance == null || !hasProcessed) return;

        if (info.effectivePlayState == PlayState.Paused)
        {
            switch (postPlayback)
            {
                case ActivationControlPlayable.PostPlaybackState.Active:
                    UIMain.Instance.Show = true;
                    break;
                case ActivationControlPlayable.PostPlaybackState.Inactive:
                    UIMain.Instance.Show = false;
                    break;
                case ActivationControlPlayable.PostPlaybackState.Revert:
                    UIMain.Instance.Show = initialActiveState;
                    break;
            }
        }

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.MusicMute(false);
        }
    }

    public override void OnPlayableDestroy(Playable playable)
    {
        hasProcessed = false;
    }
}