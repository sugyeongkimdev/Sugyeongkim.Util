using System.Linq;
using SugyeongKim.Util;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

// Component에서 자기를 기준으로 컴포넌트를 검색하는 확장 클래스
// 보통 (class Script : MonoBehaviour) 내부에서 호출을 상정, 이 경우 Component에서 호출 처리됨

// namespace SugyeongKim.Util 처리 안함
public static class HierarchySearchExtension
{
    // 하이어라이키(Hierarchy) target 기준으로 경로로 컴포넌트 찾기 (Transform.Find)
    // ex) Image searchImg = gameObject.Search<Image>("child1/innerImage");
    public static T Search<T> (this Component target, string path = "", bool alert = true) where T : Component
    {
        Transform trans = target.transform.Find (path);
        if (trans == false)
        {
            if (alert)
            {
                UtilLog.Error ($"Search fail : (\"{target}\") {path}");
            }
            return null;
        }
        if (trans.TryGetComponent (out T comp) == false)
        {
            if (alert)
            {
                UtilLog.Error ($"GetComponent fail : (\"{target}\") {path}");
            }
            return null;
        }

        return comp;
    }
    public static T Search<T> (this GameObject target, string path = "", bool alert = true) where T : Component
    {
        return target.transform.Search<T> (path, alert);
    }

    // 하이어라이키(Hierarchy) target 기준으로 경로로 게임 오브젝트 찾기 (Transform.Find)
    // ex) GameObject searchGo = gameObject.Search("child1/innerObject");
    public static GameObject Search (this Component target, string path = "")
    {
        return target.Search<Transform> (path).gameObject;
    }
    public static GameObject Search (this GameObject target, string path = "")
    {
        return target.transform.Search<Transform> (path).gameObject;
    }

    //============================================//

    // 컴포넌트 배열 찾기
    // ex1) Image[] searchImgArr = this.SearchArr<Image>("parent/childImg_1", "parent/childImg_2", "parent/childImg_3");
    // ex2) Image[] searchImgArr = gameObject.SearchArr<Image>("parent/childImg_1", "parent/childImg_2", "parent/childImg_3");
    public static T[] SearchArr<T> (this Component target, params string[] pathArr) where T : Component
    {
        return pathArr
            .Select (path => target.Search<T> (path))
            .ToArray ();
    }
    public static T[] SearchArr<T> (this GameObject target, params string[] pathArr) where T : Component
    {
        return target.transform.SearchArr<T> (pathArr);
    }
    // 게임오브젝트 배열 찾기
    // ex1) GameObject[] searchGoArr = this.SearchArr("parent/child_1", "parent/child_2", "parent/child_3");
    // ex2) GameObject[] searchGoArr = gameObject.SearchArr("parent/child_1", "parent/child_2", "parent/child_3");
    public static GameObject[] SearchArr (this Component target, params string[] pathArr)
    {
        return target.SearchArr<Transform> ()
            .Select (search => search.gameObject)
            .ToArray ();
    }
    public static GameObject[] SearchArr (this GameObject target, params string[] pathArr)
    {
        return target.transform.SearchArr (pathArr);
    }

    //============================================//
    // Range의 fomatPath 경로 양식 (택1)
    //  "aa/bb{0}/cc"
    // @"aa/bb{0}/cc"
    // $"aa/bb{{0}}/cc"

    // 지정된 숫자로 컴포넌트 범위 찾기
    // ex) "parent/child_1", "parent/child_2", "parent/child_3" ... "parent/child_10"
    // ex1) Image[] searchImgArr = this.SearchRange<Image>("parent/child_{0}", 1, 10);
    // ex2) Image[] searchImgArr = gameObject.SearchRange<Image>("parent/child_{0}", 1, 10);
    public static T[] SearchRange<T> (this Component target, string fomatPath, int start, int count) where T : Component
    {
        return Enumerable
            .Range (start, count)
            .Select (index => string.Format (fomatPath, index))
            .Select (path => target.Search<T> (path))
            .ToArray ();
    }
    public static T[] SearchRange<T> (this GameObject target, string fomatPath, int start, int count) where T : Component
    {
        return target.transform.SearchRange<T> (fomatPath, start, count);
    }
    // 지정된 숫자로 게임오브젝트 범위 찾기
    // ex) "parent/child_1", "parent/child_2", "parent/child_3" ... "parent/child_10"
    // ex1) GameObject[] searchGoArr = this.SearchRange("parent/child_{0}", 1, 10);
    // ex2) GameObject[] searchGoArr = gameObject.SearchRange("parent/child_{0}", 1, 10);
    public static GameObject[] SearchRange (this Component target, string fomatPath, int start, int count)
    {
        return target
            .SearchRange<Transform> (fomatPath, start, count)
            .Select (trans => trans.gameObject)
            .ToArray ();
    }
    public static GameObject[] SearchRange (this GameObject target, string fomatPath, int start, int count)
    {
        return target.transform.SearchRange (fomatPath, start, count);
    }

    //============================================//
    // 버튼 찾기
    public static Button SearchButton (this GameObject target, string path = "") => target.Search<Button> (path);
    public static Button SearchButton (this Component target, string path = "") => target.Search<Button> (path);
    // 이미지 찾기
    public static Image SearchImage (this GameObject target, string path = "") => target.Search<Image> (path);
    public static Image SearchImage (this Component target, string path = "") => target.Search<Image> (path);
    public static RawImage SearchRawImage (this GameObject target, string path = "") => target.Search<RawImage> (path);
    public static RawImage SearchRawImage (this Component target, string path = "") => target.Search<RawImage> (path);

}
