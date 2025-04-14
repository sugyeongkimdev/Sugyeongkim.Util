using SugyeongKim.Util;
using System;
using UniRx;

// Bootstrap 구현
public class BootstrapScene : BootstrapBase
{
    public override string NextSceneName => "IntroScene";

    private void Start ()
    {
        BootstrapAsObservable ()
            .Where (isMoveNext => isMoveNext)
            // Bootstarb scene -> Next Scene
            .SelectMany (_ => SceneControlManager.LoadSceneAsObservable (
                loadScene: NextSceneName,
                inObservable: TransitionManager.instance.FadeIn (),
                outObservable: TransitionManager.instance.FadeOut ()
            ))
            .Subscribe ();
    }

    //============================================//

    protected override IObservable<Unit> OnBootstrapAsObservable ()
    {
        return base.OnBootstrapAsObservable();
    }
}
