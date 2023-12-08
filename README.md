
## 필수
> + [UniRx](https://github.com/neuecc/UniRx)   
>   + [ump(package)](https://github.com/neuecc/UniRx#upm-package) - `https://github.com/neuecc/UniRx.git?path=Assets/Plugins/UniRx/Scripts`
> + [UniTask](https://github.com/Cysharp/UniTask)   
>   + [ump(package)](https://github.com/Cysharp/UniTask#upm-package) - `https://github.com/Cysharp/UniTask.git?path=src/UniTask/Assets/Plugins/UniTask`
<hr/>

# 패키지 추가방법

### 1. Git Submodule(subtree)
##### Git path  
`https://github.com/sugyeongkimdev/Sugyeongkim.Util.git`
##### Asset path  
`Assets/sugyeongkim.Util`

![Submodule(subtree)1](https://github.com/sugyeongkimdev/Sugyeongkim.Util/assets/51020780/ce3d2f01-fc9d-428a-81c9-291a5cd90ea9)
![Submodule(subtree)2](https://github.com/sugyeongkimdev/Sugyeongkim.Util/assets/51020780/51cfc839-49d1-4b2d-a639-32bb97686f01)
![Submodule(subtree)3](https://github.com/sugyeongkimdev/Sugyeongkim.Util/assets/51020780/df4ab3ac-02de-41f4-893a-a3e82d3c6b15)


<hr/>

# or

### 2. Add Unity package Git URL  
##### Package path  
`https://github.com/sugyeongkimdev/Sugyeongkim.Util.git?path=sugyeongkim.Util`

![git url add1](https://github.com/sugyeongkimdev/Sugyeongkim.Util/assets/51020780/20f483a5-52dd-4e01-aee9-3967fc70df84)  
![git url add2](https://github.com/sugyeongkimdev/Sugyeongkim.Util/assets/51020780/f8cc1ff5-3306-42a6-b1ad-f2b808c3b965)  

---
---

# 패키지에서 제외됨
#### UtilNGUI.cs
NUGI 유틸 코드 모음집
1. 여러개의 UIToggle에 이벤트 등록할 때 지정된 값을 등록 및 초기화 지정
2. UISprite, UILabel에 Gray Color 효과

---
---

### [UGUI]

#### UtilCommaInputField.cs
UGUI의 InputField에 숫자 입력완료시 단위마다 ","를 붙여주는 최소량의 코드

![UGUI_CommaInputField](https://user-images.githubusercontent.com/51020780/132855694-d845241f-8a02-443c-9b48-b4890c5a9d45.gif)

---
---

### [Tween]

#### MoveToTarget.cs
해당 script를 부착한 GameObject는 지정돤 타겟을 향해 설정된 값을 사용하여 이동함

![A1](https://user-images.githubusercontent.com/51020780/132530113-cd4a0359-dab2-44af-945e-fdc83552b10f.PNG)
![A2](https://user-images.githubusercontent.com/51020780/132528715-696f71be-1c34-4609-b85f-ad4b3b08743e.gif)
![A3](https://user-images.githubusercontent.com/51020780/132528724-4f9569d8-b42e-4739-8268-4070b881861c.gif)

#

#### MoveToStepTarget.cs
MoveToTarget.cs를 상속받아서 작성된 경로 애니메이션 코드

![VFX_MoveToStepTargetInspector](https://user-images.githubusercontent.com/51020780/133119919-bdb47d3f-f0b0-4271-84a6-064a4637bf08.PNG)
![VFX_MoveToStepTarget1](https://user-images.githubusercontent.com/51020780/133119908-1d5633a5-8e8a-4145-8d83-f6a818a23eee.gif)
![VFX_MoveToStepTarget2](https://user-images.githubusercontent.com/51020780/133119913-5b73fa93-f577-4d1e-9389-d2079096e001.gif)
![VFX_MoveToStepTarget3](https://user-images.githubusercontent.com/51020780/133119914-7f65cf2b-bfd9-4d2a-9ca5-f423459a73d3.gif)

---
---

### [Util]

#### UtilLog.cs
Debug.Log / Debug.Error
색상 및 호출자 로그
![console](https://user-images.githubusercontent.com/51020780/132694076-70d4d95c-8dae-4b76-b496-e9b8d069f4c1.PNG)

#

#### ScriptExtension.cs
코드 편의성 확장 모음집
1. IEnumerable을 상속받는 모든 열거자에대한 반복문 (Linq.ForEach는 List만 지원함)

#

#### InspectorAttribute.cs
Inspector 간단 확장 에디터  
나중에 손좀 봐야함

![inspector](https://user-images.githubusercontent.com/51020780/132705061-77178987-0d33-4cdc-8c20-89e90263fb6f.png)

#

#### UtilSingleton.cs
통합 싱글톤 코드 작성, 자세한 내용은 코드 및 스크린샷 참조
1. 초기화와 UniRx 초기화를 지원함
1. GlobalSingleton을 상속받으면 위치불문하고 검색 및 instance를 생성, 매니저급 클래스에서 사용하면 됨
2. LocalSingleton를 상속받으면 평범한 싱글톤 기능을 하지만 instance를 찾기만 하며 생명주기는 Scene에 한정됨.

![singleton0](https://github.com/sugyeongkimdev/Sugyeongkim.BigUtil.Unity/assets/51020780/07beb0de-671d-4707-8cfb-22576185ef17)
![singleton1](https://github.com/sugyeongkimdev/Sugyeongkim.BigUtil.Unity/assets/51020780/d3c320e0-944f-4c0d-81c2-554fe9720709)
![singleton2](https://github.com/sugyeongkimdev/Sugyeongkim.BigUtil.Unity/assets/51020780/6e4e6ed4-08f9-4cdc-b480-9d5a61e8f8af)

#

#### UtilKeyInput.cs

아주 간단한 키 입력 이벤트 등록 (동시 입력 가능)  
관리자로 쓸거면 싱글톤으로 만들어서 쓰거나, Linq가 싫거나 성능이 중요하면 Loop부분을 입맛대로 수정하면 됨

![Input](https://user-images.githubusercontent.com/51020780/132979324-5b4ee554-b138-4cb7-8134-53d9e3d5ce79.PNG)
![InputGIF](https://user-images.githubusercontent.com/51020780/132979325-f5a18fa2-ccd8-4ef2-8517-bce11e0bf177.gif)

---
---

### [UniRx 유틸]

#### AnimationEventSubject.cs
해당 script를 부착한 unity animation은 UniRx의 Subject로 이벤트를 등록하여서 결합하기 쉬워짐
