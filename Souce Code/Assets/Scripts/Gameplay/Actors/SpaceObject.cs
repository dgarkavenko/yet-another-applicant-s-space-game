using UnityEngine;
using System.Collections;

public class SpaceObject : MonoBehaviour {

    public Vector2 Size;
    
        
    protected void RecalculateSizeByCollider()
    {

        Size = Vector2.zero;

        var colliders = GetComponentsInChildren<Collider2D>();
        foreach (var collider in colliders)
        {
            if (Size.x < collider.bounds.size.x)
                Size.x = collider.bounds.size.x;

            if (Size.y < collider.bounds.size.y)
                Size.y = collider.bounds.size.y;
        }
    }

    protected void RecalculateSizeBySprite()
    {
        Size = Vector2.zero;

        var renderes = GetComponentsInChildren<SpriteRenderer>();
        foreach (var renderer in renderes)
        {
            if (Size.x < renderer.sprite.rect.width / CameraController.PIXELS_IN_METER) 
                Size.x = renderer.sprite.rect.width / CameraController.PIXELS_IN_METER;

            if (Size.y < renderer.sprite.rect.height / CameraController.PIXELS_IN_METER)
                Size.y = renderer.sprite.rect.height / CameraController.PIXELS_IN_METER;
        }
    }
}
