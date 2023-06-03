# This is submodule git

### Root git : https://github.com/sugyeongkimdev/Sugyeongkim.BigUtil  

---

### VFX_MoveToTarget.cs
해당 script를 부착한 GameObject는 지정돤 타겟을 향해 설정된 값을 사용하여 이동함

![A1](https://user-images.githubusercontent.com/51020780/132530113-cd4a0359-dab2-44af-945e-fdc83552b10f.PNG)
![A2](https://user-images.githubusercontent.com/51020780/132528715-696f71be-1c34-4609-b85f-ad4b3b08743e.gif)
![A3](https://user-images.githubusercontent.com/51020780/132528724-4f9569d8-b42e-4739-8268-4070b881861c.gif)

#

### VFX_MoveToStepTarget.cs
VFX_MoveToTarget.cs를 상속받아서 작성된 경로 애니메이션 코드

![VFX_MoveToStepTargetInspector](https://user-images.githubusercontent.com/51020780/133119919-bdb47d3f-f0b0-4271-84a6-064a4637bf08.PNG)
![VFX_MoveToStepTarget1](https://user-images.githubusercontent.com/51020780/133119908-1d5633a5-8e8a-4145-8d83-f6a818a23eee.gif)
![VFX_MoveToStepTarget2](https://user-images.githubusercontent.com/51020780/133119913-5b73fa93-f577-4d1e-9389-d2079096e001.gif)
![VFX_MoveToStepTarget3](https://user-images.githubusercontent.com/51020780/133119914-7f65cf2b-bfd9-4d2a-9ca5-f423459a73d3.gif)

#

### Console.cs
Debug.Log / Debug.Error 간편한 확장법

![console](https://user-images.githubusercontent.com/51020780/132694076-70d4d95c-8dae-4b76-b496-e9b8d069f4c1.PNG)

#

### ScriptExtension.cs
코드 편의성 확장 모음집
1. IEnumerable을 상속받는 모든 열거자에대한 반복문 (Linq.ForEach는 List만 지원함)

#

### InspectorAttribute.cs
Inspector 간단 확장 에디터  
나중에 손좀 봐야함

![inspector](https://user-images.githubusercontent.com/51020780/132705061-77178987-0d33-4cdc-8c20-89e90263fb6f.png)

#

### [UGUI 유틸](Script/UGUI)

### UGUI_CommaInputField.cs
UGUI의 InputField에 숫자 입력완료시 단위마다 ","를 붙여주는 최소량의 코드

![UGUI_CommaInputField](https://user-images.githubusercontent.com/51020780/132855694-d845241f-8a02-443c-9b48-b4890c5a9d45.gif)

#

### Singleton.cs
통합 싱글톤 코드 작성, 자세한 내용은 코드 및 스크린샷 참조
1. GlobalSingleton을 상속받으면 위치불문하고 검색 및 instance를 생성하며 초기화와 async초기화를 지원함.
   매니저급 클래스에서 사용하면 됨
2. SimpleSingleton를 상속받으면 평범한 싱글톤 기능을 하지만 instance를 찾기만 하며 생명주기는 Scene에 한정됨.

![A](https://user-images.githubusercontent.com/51020780/132859707-a45395ec-7e3e-454d-8e8a-3616d6f354b5.PNG)
![B](https://user-images.githubusercontent.com/51020780/132859711-78b42159-3412-4d8b-beac-81537ce62629.PNG)
![D](https://user-images.githubusercontent.com/51020780/132859713-5c0371b3-610f-4f04-9337-10d7cd84185c.PNG)

#

### KeyInput.cs

아주 간단한 키 입력 이벤트 등록 (동시 입력 가능)  
관리자로 쓸거면 싱글톤으로 만들어서 쓰거나, Linq가 싫거나 성능이 중요하면 Loop부분을 입맛대로 수정하면 됨

![Input](https://user-images.githubusercontent.com/51020780/132979324-5b4ee554-b138-4cb7-8134-53d9e3d5ce79.PNG)
![InputGIF](https://user-images.githubusercontent.com/51020780/132979325-f5a18fa2-ccd8-4ef2-8517-bce11e0bf177.gif)
