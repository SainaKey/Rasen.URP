using UnityEngine;

public class CameraBlit : MonoBehaviour
{
    [SerializeField] private Shader shader;
    [SerializeField] private Material material;
    [SerializeField] private RenderTexture inRT;
    [SerializeField] private RenderTexture outRT;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        material = new Material(shader);
    }

    // Update is called once per frame
    void Update()
    {
        Graphics.Blit(inRT, outRT, material);
    }
}
