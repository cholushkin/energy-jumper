// using CGTespy.UI;
// using GameLib;
// using UnityEngine;
// using UnityEngine.Assertions;
// using UnityEngine.UI;
//
// namespace Game
// {
//     public class DbgInputGameAction : DebugLayoutElement
//     {
//         protected Text _textAxis1;
//         protected Text _textAxis2;
//         protected Text _textAxis3;
//         protected Text _textAcceleration;
//
//         protected Image _imagePointer1;
//         protected Image _imagePointer2;
//         protected Image _imagePointer3;
//
//         public override string GetPrefabBasedOnName()
//         {
//             return "DbgInputGameAction";
//         }
//
//         public override void InitializeState()
//         {
//             base.InitializeState();
//
//             _textAxis1 = transform.Find("TextAxis1").GetComponent<Text>();
//             _textAxis2 = transform.Find("TextAxis2").GetComponent<Text>();
//             _textAxis3 = transform.Find("TextAxis3").GetComponent<Text>();
//
//             _imagePointer1 = transform.Find("ImagePointer1").GetComponent<Image>();
//             _imagePointer2 = transform.Find("ImagePointer2").GetComponent<Image>();
//             _imagePointer3 = transform.Find("ImagePointer3").GetComponent<Image>();
//
//
//             _textAcceleration = transform.Find("TextAcceleration").GetComponent<Text>();
//             _textAxis1.text = _textAxis2.text =
//                 _textAxis3.text = _textAcceleration.text = "";
//
//             Assert.IsNotNull(_textAxis1);
//             Assert.IsNotNull(_textAxis2);
//             Assert.IsNotNull(_textAxis3);
//             Assert.IsNotNull(_textAcceleration);
//             Assert.IsNotNull(_imagePointer1);
//             Assert.IsNotNull(_imagePointer2);
//             Assert.IsNotNull(_imagePointer3);
//         }
//
//
//         public override void Update()
//         {
//             base.Update();
//             _textAxis1.text = InputHandler.Instance.MoveVector.ToString();
//             _textAxis2.text = InputHandler.Instance.AimingDirection.ToString();
//             //_textAcceleration.text = $"Acceleration:\n{Input.acceleration}";
//
//             var w = GetComponent<RectTransform>().Width() / 2;
//             var h = GetComponent<RectTransform>().Height() / 2;
//
//             _imagePointer1.GetComponent<RectTransform>().anchoredPosition = new Vector2(
//                 InputHandler.Instance.MoveVector.x * w,
//                 InputHandler.Instance.MoveVector.y * h);
//
//             _imagePointer2.GetComponent<RectTransform>().anchoredPosition = new Vector2(
//                 InputHandler.Instance.AimingDirection.x * w,
//                 InputHandler.Instance.AimingDirection.y * h);
//
//
//             //var xf = Input.GetAxis("Horizontal");
//             //var yf = Input.GetAxis("Vertical");
//             //var normDirection = new Vector2(xf, yf).normalized;
//             //var maxx = normDirection.x; // projection of x component to horizontal axis (maximum value with a sign for curent direction)
//             //var maxy = normDirection.y;
//
//             //_imagePointerCircled.GetComponent<RectTransform>().anchoredPosition = new Vector2(
//             //    maxx * Mathf.Abs(xf) * w, // Mathf.Abs(xf) here is percent of propagation on current axis
//             //    maxy * Mathf.Abs(yf) * h);
//         }
//     }
// }
//
