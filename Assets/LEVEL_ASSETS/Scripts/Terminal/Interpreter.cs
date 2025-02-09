using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;
using UnityEngine.UI;
using System.Threading;
using Unity.VisualScripting;

public class Interpreter : MonoBehaviour {


    public GameObject platformPrefab;
    public GameObject wallPrefab;
    public GameObject lineFX;
    public GameObject currPlatform;
    public PlayerAnimator AnimHandler;
    public AudioSource cmdSound;
    public bool godmodeUnlocked;

    public AudioSource[] glitchRandom;
    public AudioSource[] glitchRepeat;
    public AudioSource[] glitchBlip;
    public GameObject _player;
    private float lastShiftTime = 0f;

    List<string> response = new();

    private void Start() {
    }

    GameObject FindClosestGameObjectWithName(string nameFragment)
    {
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
        GameObject closestObject = null;
        float closestDistance = 15f;

        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains(nameFragment) && obj.activeInHierarchy)
            {
                float distance = Mathf.Pow(Mathf.Pow(_player.transform.position.x - obj.transform.position.x, 2) + Mathf.Pow(_player.transform.position.y - obj.transform.position.y, 2), 0.5f);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestObject = obj;
                }
            }
        }

        return closestObject;
    }

    private void Update() {
        if (godmodeUnlocked) {
            if (Input.GetKeyDown(KeyCode.LeftShift) && Time.time - lastShiftTime >= 0.1) { // add temp platform
                lastShiftTime = Time.time;
                AnimHandler.doCommand = true;
                AnimHandler.playerText.text = "$ sudo add --platform -1 0";
                GameObject spriteObject = GameObject.Find("Player").transform.Find("Square").gameObject;
                currPlatform = Instantiate(platformPrefab, spriteObject.transform.position + new Vector3(0, -1, 0), Quaternion.Euler(0, 0, 0));
                currPlatform.name = "temp";
                StartCoroutine(animateAdd(currPlatform));
            }
            if (Input.GetKeyUp(KeyCode.LeftShift)) {
                if (!currPlatform.IsDestroyed()) {
                    StartCoroutine(zDestroy(currPlatform));
                }
            }
            if (Input.GetKeyDown(KeyCode.X)) { // destroy other platform
                AnimHandler.doCommand = true;
                GameObject other = FindClosestGameObjectWithName("%");
                AnimHandler.playerText.text = "$ sudo destroy % 1";
                if (other != null) {
                    StartCoroutine(xDestroy(other));
                }
            }
        }
    }
    


    IEnumerator animateAdd(GameObject newPlatform) {
        List<GameObject> FXList = new();
        glitchBlip[Random.Range(0, glitchBlip.Length)].Play();
        GameObject spriteObject = GameObject.Find("Player").transform.Find("Square").gameObject;
        newPlatform.GetComponent<SpriteRenderer>().color = spriteObject.GetComponent<SpriteRenderer>().color;
        Vector3[] rel_corners = new Vector3[4];
        newPlatform.GetComponent<RectTransform>().GetWorldCorners(rel_corners);

        for (int j = 0; j < rel_corners.Length; j++) {
            GameObject line = Instantiate(lineFX, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0));
            line.GetComponent<LineRenderer>().startColor = spriteObject.GetComponent<SpriteRenderer>().color;
            line.GetComponent<LineRenderer>().endColor = spriteObject.GetComponent<SpriteRenderer>().color;
            Vector3[] points = new Vector3[] {spriteObject.transform.position, rel_corners[j]};
            line.GetComponent<LineRenderer>().SetPositions(points);
            FXList.Add(line);
        }

        Destroy(FXList[2]);

        yield return new WaitForSeconds(0.04f);
        newPlatform.SetActive(false);
        Destroy(FXList[1]);
        Destroy(FXList[3]);
        yield return new WaitForSeconds(0.02f);
        newPlatform.SetActive(true);
        yield return new WaitForSeconds(0.018f);
        newPlatform.SetActive(false);
        Destroy(FXList[0]);
        yield return new WaitForSeconds(0.07f);
        newPlatform.SetActive(true);
    }

    IEnumerator animateDestroy(GameObject rel) {
        List<GameObject> FXList = new();
        SpriteRenderer rel_spriteRenderer = rel.GetComponent<SpriteRenderer>();
        int index = Random.Range(0, glitchRandom.Length);
        glitchRandom[index].Play();
        GameObject spriteObject = GameObject.Find("Player").transform.Find("Square").gameObject;
        rel_spriteRenderer.color = spriteObject.GetComponent<SpriteRenderer>().color;
        Vector3[] rel_corners = new Vector3[4];
        rel.GetComponent<RectTransform>().GetWorldCorners(rel_corners);

        for (int j = 0; j < rel_corners.Length; j++) {
            yield return new WaitForSeconds(0.02f);
            GameObject line = Instantiate(lineFX, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0));
            line.GetComponent<LineRenderer>().startColor = spriteObject.GetComponent<SpriteRenderer>().color;
            line.GetComponent<LineRenderer>().endColor = spriteObject.GetComponent<SpriteRenderer>().color;
            Vector3[] points = new Vector3[] {spriteObject.transform.position, rel_corners[j]};
            line.GetComponent<LineRenderer>().SetPositions(points);
            FXList.Add(line);
        }
        yield return new WaitForSeconds(0.02f);
        for (int j = 1; j < 11; j++) {
            GameObject line = Instantiate(lineFX, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0));
            line.GetComponent<LineRenderer>().startColor = spriteObject.GetComponent<SpriteRenderer>().color;
            line.GetComponent<LineRenderer>().endColor = spriteObject.GetComponent<SpriteRenderer>().color;
            Vector3[] points = new Vector3[] {spriteObject.transform.position, rel.transform.position + new Vector3(Random.Range(-1.2f * j / 4, 1.2f * j / 4), Random.Range(-1.2f * Mathf.Log(j) / 5, 1.2f * Mathf.Log(j) / 5), 0)};
            line.GetComponent<LineRenderer>().SetPositions(points);

            rel.transform.localScale += new Vector3(Random.Range(-1.0f * j / 3, 1.0f * j / 5), Random.Range(-2.0f * j / 4, 2.0f * j / 4), 0);
            yield return new WaitForSeconds(2f / (j + 20));
            Destroy(line);

            if (j == 2) {
                for (int k = 2; k < FXList.Count; k++) {
                    Destroy(FXList[k]);
                }
            }
            if (j == 3) {
                Destroy(FXList[0]);
            }
            if (j == 4) {
                Destroy(FXList[1]);
            }
        //     // for (int k = 0; k < rel_spriteVertices.Length; k++) {
        //     //     rel_spriteVertices[k].x += Random.Range(-1.0f, 1.0f);
        //     //     rel_spriteVertices[k].y += Random.Range(-1.0f, 1.0f);
        //     // }

        //     // for (int k = 0; k < rel_spriteVertices.Length; k++) {
        //     //     rel_spriteVertices[k] = (rel_spriteVertices[k] * rel_sprite.pixelsPerUnit) + rel_sprite.pivot;
        //     // }
        //     rel_sprite.OverrideGeometry(rel_spriteVertices, rel_sprite.triangles);
            // yield return new WaitForSeconds(0.2f);
        }
        FXList.Clear();

        Destroy(rel);

        glitchRandom[index].Stop();
    }

    IEnumerator zDestroy(GameObject rel) {
        List<GameObject> FXList = new();
        SpriteRenderer rel_spriteRenderer = rel.GetComponent<SpriteRenderer>();
        int index = Random.Range(0, glitchRandom.Length);
        glitchRandom[index].Play();
        GameObject spriteObject = GameObject.Find("Player").transform.Find("Square").gameObject;
        rel_spriteRenderer.color = spriteObject.GetComponent<SpriteRenderer>().color;
        Vector3[] rel_corners = new Vector3[4];
        rel.GetComponent<RectTransform>().GetWorldCorners(rel_corners);

        for (int j = 0; j < rel_corners.Length; j++) {
            GameObject line = Instantiate(lineFX, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0));
            line.GetComponent<LineRenderer>().startColor = spriteObject.GetComponent<SpriteRenderer>().color;
            line.GetComponent<LineRenderer>().endColor = spriteObject.GetComponent<SpriteRenderer>().color;
            Vector3[] points = new Vector3[] {spriteObject.transform.position, rel_corners[j]};
            line.GetComponent<LineRenderer>().SetPositions(points);
            FXList.Add(line);
        }
        yield return new WaitForSeconds(0.01f);
        for (int j = 1; j < 5; j++) {
            GameObject line = Instantiate(lineFX, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0));
            line.GetComponent<LineRenderer>().startColor = spriteObject.GetComponent<SpriteRenderer>().color;
            line.GetComponent<LineRenderer>().endColor = spriteObject.GetComponent<SpriteRenderer>().color;
            Vector3[] points = new Vector3[] {spriteObject.transform.position, rel.transform.position + new Vector3(Random.Range(-1.2f * j / 4, 1.2f * j / 4), Random.Range(-1.2f * Mathf.Log(j) / 5, 1.2f * Mathf.Log(j) / 5), 0)};
            line.GetComponent<LineRenderer>().SetPositions(points);

            rel.transform.localScale += new Vector3(Random.Range(-1.0f * j / 5, 1.0f * j / 5), Random.Range(-1.0f * j / 5, 1.0f * j / 5), 0);
            yield return new WaitForSeconds(1f / (j + 20));
            Destroy(line);

            if (j == 2) {
                for (int k = 2; k < FXList.Count; k++) {
                    Destroy(FXList[k]);
                }
            }
            if (j == 3) {
                Destroy(FXList[0]);
            }
            if (j == 4) {
                Destroy(FXList[1]);
            }
        //     // for (int k = 0; k < rel_spriteVertices.Length; k++) {
        //     //     rel_spriteVertices[k].x += Random.Range(-1.0f, 1.0f);
        //     //     rel_spriteVertices[k].y += Random.Range(-1.0f, 1.0f);
        //     // }

        //     // for (int k = 0; k < rel_spriteVertices.Length; k++) {
        //     //     rel_spriteVertices[k] = (rel_spriteVertices[k] * rel_sprite.pixelsPerUnit) + rel_sprite.pivot;
        //     // }
        //     rel_sprite.OverrideGeometry(rel_spriteVertices, rel_sprite.triangles);
            // yield return new WaitForSeconds(0.2f);
        }
        FXList.Clear();

        rel.SetActive(false);

        glitchRandom[index].Stop();
        yield return new WaitForSeconds(2f);
        Destroy(rel);
    }

    IEnumerator xDestroy(GameObject rel) {
        Vector3 opos = rel.transform.position;
        List<GameObject> FXList = new();
        SpriteRenderer rel_spriteRenderer = rel.GetComponent<SpriteRenderer>();
        int index = Random.Range(0, glitchRandom.Length);
        glitchRandom[index].Play();
        GameObject spriteObject = GameObject.Find("Player").transform.Find("Square").gameObject;
        rel_spriteRenderer.color = spriteObject.GetComponent<SpriteRenderer>().color;
        Vector3[] rel_corners = new Vector3[4];
        rel.GetComponent<RectTransform>().GetWorldCorners(rel_corners);

        for (int j = 0; j < rel_corners.Length; j++) {
            GameObject line = Instantiate(lineFX, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0));
            line.GetComponent<LineRenderer>().startColor = spriteObject.GetComponent<SpriteRenderer>().color;
            line.GetComponent<LineRenderer>().endColor = spriteObject.GetComponent<SpriteRenderer>().color;
            Vector3[] points = new Vector3[] {spriteObject.transform.position, rel_corners[j]};
            line.GetComponent<LineRenderer>().SetPositions(points);
            FXList.Add(line);
        }
        yield return new WaitForSeconds(0.01f);
        for (int j = 1; j < 11; j++) {
            GameObject line = Instantiate(lineFX, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0));
            line.GetComponent<LineRenderer>().startColor = spriteObject.GetComponent<SpriteRenderer>().color;
            line.GetComponent<LineRenderer>().endColor = spriteObject.GetComponent<SpriteRenderer>().color;
            Vector3[] points = new Vector3[] {spriteObject.transform.position, rel.transform.position + new Vector3(Random.Range(-1.2f * j / 4, 1.2f * j / 4), Random.Range(-1.2f * Mathf.Log(j) / 5, 1.2f * Mathf.Log(j) / 5), 0)};
            line.GetComponent<LineRenderer>().SetPositions(points);

            rel.transform.localScale += new Vector3(Random.Range(-1.0f * j / 4, 1.0f * j / 4), Random.Range(-1.0f * j / 4, 1.0f * j / 4), 0);
            rel.transform.position = opos;
            yield return new WaitForSeconds(0.9f / (j + 20));
            Destroy(line);

            if (j == 2) {
                for (int k = 2; k < FXList.Count; k++) {
                    Destroy(FXList[k]);
                }
            }
            if (j == 3) {
                Destroy(FXList[0]);
            }
            if (j == 4) {
                Destroy(FXList[1]);
            }
        //     // for (int k = 0; k < rel_spriteVertices.Length; k++) {
        //     //     rel_spriteVertices[k].x += Random.Range(-1.0f, 1.0f);
        //     //     rel_spriteVertices[k].y += Random.Range(-1.0f, 1.0f);
        //     // }

        //     // for (int k = 0; k < rel_spriteVertices.Length; k++) {
        //     //     rel_spriteVertices[k] = (rel_spriteVertices[k] * rel_sprite.pixelsPerUnit) + rel_sprite.pivot;
        //     // }
        //     rel_sprite.OverrideGeometry(rel_spriteVertices, rel_sprite.triangles);
            // yield return new WaitForSeconds(0.2f);
        }
        FXList.Clear();

        rel.SetActive(false);

        glitchRandom[index].Stop();
    }
}
