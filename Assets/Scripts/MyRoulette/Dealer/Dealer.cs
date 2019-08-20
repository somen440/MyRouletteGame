using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class Dealer : MonoBehaviour
{
    [SerializeField] InputField betInputField = null;
    [SerializeField] InputField forecastInputField = null;
    [SerializeField] Button runButton = null;
    [SerializeField] Text stopText = null;
    [SerializeField] Text hitText = null;

    private const string STOP_BASE = "num: {0}";
    private const string HIT_BASE = "hit: {0}";

    private int betNum = 0;
    private int forecastNum = 0;

    private BoolReactiveProperty isBetEndEdit = new BoolReactiveProperty(false);
    private BoolReactiveProperty isForecastEndEdit = new BoolReactiveProperty(false);
    private Wheel wheel = null;

    // Start is called before the first frame update
    void Start()
    {
        betInputField.OnEndEditAsObservable()
            .Where(x => x != null && !string.IsNullOrWhiteSpace(x))
            .Delay(TimeSpan.FromSeconds(1))
            .Subscribe(x =>
            {
                // 文字列への変数埋め込み
                // @see https://blog.hiros-dot.net/?p=6407
                Debug.Log($"bet is {x}");
                betNum = int.Parse(x);
                isBetEndEdit.Value = true;
            });

        forecastInputField.OnEndEditAsObservable()
            .Where(x => x != null && !string.IsNullOrWhiteSpace(x))
            .Delay(TimeSpan.FromSeconds(1))
            .Subscribe(x =>
            {
                Debug.Log($"forecast is {x}");
                forecastNum = int.Parse(x);
                isForecastEndEdit.Value = true;
            });

        wheel = GameObject.Find("Wheel").GetComponent<Wheel>();

        var run = isBetEndEdit
            .CombineLatest(isForecastEndEdit, (x, y) => x & y)
            .ToReactiveCommand();
        run.Subscribe(_ => {
            Debug.Log("wheel run start");
            betInputField.enabled = forecastInputField.enabled = false;
            isBetEndEdit.Value = isForecastEndEdit.Value = false;
            wheel.Run();
        });
        run.BindTo(runButton);

        // memo:
        // meta ファイルから executionOrder で読み込み順を Wheel より後にする必要あり
        // wheel 側で ?.State ?? new ReactiveProperty* とやった場合, 参照エラーは回避できる
        // ただし,　そうすると new の方を参照するため Wheel が持っている IRollable の State は購買できない
        //
        // 追記:
        // Wheel 側で IRollable.State を購買して自身の state を更新するという処理で meta ファイルをイジる事なく回避
        wheel.State
            .Where(x => x == RollState.Stoped)
            .Subscribe(x =>
            {
                Debug.Log("wheel stoped");
                betInputField.text = forecastInputField.text = "";
                betInputField.enabled = forecastInputField.enabled = true;
                if (forecastNum == wheel.ClosestNumber.Value)
                {
                    hitText.text = string.Format(HIT_BASE, "当たり！！");
                }
                else
                {
                    hitText.text = string.Format(HIT_BASE, "ハズレ...");
                }
            });
        wheel.ClosestNumber
            .Where(x => x != 0)
            .Subscribe(x =>
            {
                stopText.text = string.Format(
                    STOP_BASE,
                    x
                );
            });

        Debug.Log("Dealer Start End");
    }
}
