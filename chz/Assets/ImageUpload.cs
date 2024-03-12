using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ImageUpload : MonoBehaviour,IPointerClickHandler
{
    public RawImage imageDisplay;

    
    public void SelectImage()
    {
        // 파일 대화 상자 열기
        string imagePath = UnityEditor.EditorUtility.OpenFilePanel("Select Image", "", "png,jpg,jpeg");

        // 파일 경로가 유효한지 확인
        if (!string.IsNullOrEmpty(imagePath))
        {
            // 이미지 로드 및 표시
            LoadImageFromFile(imagePath);
        }
    }

    // 파일에서 이미지를 로드하여 표시하는 메서드
    private void LoadImageFromFile(string path)
    {
        // 파일을 바이트 배열로 읽기
        byte[] fileData = File.ReadAllBytes(path);

        // 바이트 배열을 텍스처로 변환
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(fileData);

        // 이미지 표시
        imageDisplay.texture = texture;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        SelectImage();
    }
}
