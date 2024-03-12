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
        // ���� ��ȭ ���� ����
        string imagePath = UnityEditor.EditorUtility.OpenFilePanel("Select Image", "", "png,jpg,jpeg");

        // ���� ��ΰ� ��ȿ���� Ȯ��
        if (!string.IsNullOrEmpty(imagePath))
        {
            // �̹��� �ε� �� ǥ��
            LoadImageFromFile(imagePath);
        }
    }

    // ���Ͽ��� �̹����� �ε��Ͽ� ǥ���ϴ� �޼���
    private void LoadImageFromFile(string path)
    {
        // ������ ����Ʈ �迭�� �б�
        byte[] fileData = File.ReadAllBytes(path);

        // ����Ʈ �迭�� �ؽ�ó�� ��ȯ
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(fileData);

        // �̹��� ǥ��
        imageDisplay.texture = texture;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        SelectImage();
    }
}
