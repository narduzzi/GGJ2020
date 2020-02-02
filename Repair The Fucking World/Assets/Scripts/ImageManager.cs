using UnityEngine;
using UnityEngine.UI;

public class ImageManager : MonoBehaviour
{
    public GameObject ChaosBackground;
    public GameObject NatureBackground;
    public GameObject DictatorBackground;
    public GameObject UtopiaBackground;
    public GameObject BlackBackground;
    public GameObject ExplosionBackground;

    public GameObject ExplosionAnimation;
    public GameObject BigBangAnimation;
    public GameObject PortalAnimation;

    public GameObject Chaos;
    public GameObject Antique;
    public GameObject Medieval;
    public GameObject Industry;
    public GameObject Current;

    public GameObject EarthBackground;


    public static GameObject _ChaosBackground;
    public static GameObject _NatureBackground;
    public static GameObject _DictatorBackground;
    public static GameObject _UtopiaBackground;
    public static GameObject _ExplosionBackground;
    public static GameObject _BlackBackground;

    public static GameObject _Chaos;
    public static GameObject _Antique;
    public static GameObject _Medieval;
    public static GameObject _Industry;
    public static GameObject _Current;

    public static GameObject _EarthBackground;


    public static GameObject _ExplosionAnimation;
    public static GameObject _BigBangAnimation;
    public static GameObject _PortalAnimation;

    // Start is called before the first frame update
    void Start()
    {
        _ChaosBackground = ChaosBackground;
        _NatureBackground = NatureBackground;
        _DictatorBackground = DictatorBackground;
        _UtopiaBackground = UtopiaBackground;
        _ExplosionBackground = ExplosionBackground;
        _BlackBackground = BlackBackground;

        _ExplosionAnimation = ExplosionAnimation;
        _BigBangAnimation = BigBangAnimation;
        _PortalAnimation = PortalAnimation;

        _Chaos = Chaos;
        _Antique = Antique;
        _Industry = Industry;
        _Medieval = Medieval;
        _Current = Current;

        _EarthBackground = EarthBackground;
    }
}
