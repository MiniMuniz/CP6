using UnityEngine;

public class GirarAlvo : MonoBehaviour
{
    [SerializeField] private Vector3 eixoRotacao = Vector3.up;
    [SerializeField] private float velocidadeRotacao = 90f;

    private void Update()
    {
        transform.Rotate(
            eixoRotacao.normalized,
            velocidadeRotacao * Time.deltaTime,
            Space.Self
        );
    }
}