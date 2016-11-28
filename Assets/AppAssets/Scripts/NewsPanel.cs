using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using System.Collections;

public class NewsPanel : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler, IPointerEnterHandler, IBeginDragHandler, IDragHandler, IEndDragHandler {

	public Image newsImage;
	public InputField newsTitle, newsText;
	public GameObject loadingIcon;
	public Vector2 windowMinSize;
	public Vector2 windowMaxSize;
	public float pinchZoomSpeed = 0.5f;
	public bool isFullScreenMode = false;

	[HideInInspector]
	public string englishText;
	[HideInInspector]
	public string englishTitle;
	[HideInInspector]
	public string newsURL;

	private NewsController m_newsController;
	private NewsFeedApplication m_app;

	private int m_realChildIndex;
	private float m_clickTimer;

	private Camera m_camera;

	private Vector3 m_initialPosition;
	private Vector2 m_dragOffset;

	private bool m_isDragging = false;
	private bool m_isClicking = false;

	private void Start(){
		m_camera = GameObject.FindWithTag("UICamera").GetComponent<Camera>();
	}

	private void Update(){
		if(!isFullScreenMode){
			if(m_isClicking && Input.touchCount == 2){
				Touch touchZero = Input.GetTouch(0);
				Touch touchOne = Input.GetTouch(1);

				// Find the position in the previous frame of each touch.
				Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
				Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

				// Find the magnitude of the vector (the distance) between the touches in each frame.
				float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
				float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

				// Find the difference in the distances between each frame.
				float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

				Vector2 toScale = new Vector2(gameObject.GetComponent<RectTransform>().sizeDelta.x + (-deltaMagnitudeDiff * pinchZoomSpeed), gameObject.GetComponent<RectTransform>().sizeDelta.y + (-deltaMagnitudeDiff * pinchZoomSpeed));
				if(toScale.x < windowMinSize.x){
					toScale.x = windowMinSize.x;
				}else if(toScale.x > windowMaxSize.x){
					toScale.x = windowMaxSize.x;
				}
				if(toScale.y < windowMinSize.y){
					toScale.y = windowMinSize.y;
				}else if(toScale.y > windowMaxSize.y){
					toScale.y = windowMaxSize.y;
				}

				gameObject.GetComponent<RectTransform>().sizeDelta =  toScale;
			}

			if(Input.GetKey(KeyCode.Z)){
				Vector2 toScale = new Vector2(gameObject.GetComponent<RectTransform>().sizeDelta.x + (1 * pinchZoomSpeed), gameObject.GetComponent<RectTransform>().sizeDelta.y + (1 * pinchZoomSpeed));

				if(toScale.x < windowMinSize.x){
					toScale.x = windowMinSize.x;
				}else if(toScale.x > windowMaxSize.x){
					toScale.x = windowMaxSize.x;
				}
				if(toScale.y < windowMinSize.y){
					toScale.y = windowMinSize.y;
				}else if(toScale.y > windowMaxSize.y){
					toScale.y = windowMaxSize.y;
				}

				gameObject.GetComponent<RectTransform>().sizeDelta =  toScale;
			}
		}
	}

	public void SetNewsInterface(string title, string text, string image_url, string news_url, int realIndex, NewsFeedApplication _app, NewsController newsController, bool isCloudMode){
		newsTitle.text = title;
		englishTitle = title;
		newsText.text = text;
		englishText = text;
		newsURL = news_url;
		m_realChildIndex = realIndex;
		m_app = _app;
		m_newsController = newsController;

		newsImage.gameObject.SetActive(false);

		StartCoroutine(GetNewsImage(image_url));
		iTween.ValueTo(gameObject, iTween.Hash("from",0,"to",1,"time",2f,"onupdate","FadeInPanel"));
	}

	private void FadeInPanel(float value){
		gameObject.GetComponent<CanvasGroup>().alpha = value;
	}

	private IEnumerator GetNewsImage(string url){
		WWW www = new WWW(url);
		yield return www;

		if(string.IsNullOrEmpty(www.error)){
			loadingIcon.SetActive(false);
			newsImage.gameObject.SetActive(true);

			Texture2D nTexture = www.texture;
			Sprite nSprite = Sprite.Create(nTexture, new Rect(0,0,nTexture.width, nTexture.height), new Vector2(0.5f,0.5f));
			newsImage.sprite = nSprite;
			newsImage.preserveAspect = true;
		}
	}

	public void OnPointerDown(PointerEventData ev){
		if(m_newsController.interactingPanel != null)
			return;

		if(!isFullScreenMode){
			m_app.Notify(NewsFeedNotification.NewsHoverIn, this, newsTitle.text);
			m_newsController.interactingPanel = this;		
			m_isClicking = true;
			m_clickTimer = Time.time;
		}
	}
	public void OnPointerUp(PointerEventData ev){
		if(m_newsController.interactingPanel != null && m_newsController.interactingPanel == this && !isFullScreenMode){
			m_isClicking = false;
			m_newsController.interactingPanel = null;
			float timeToClick = Time.time - m_clickTimer;
			if(timeToClick < 0.12f){
				m_app.Notify(NewsFeedNotification.NewsShowFull, this, this);
				m_app.Notify(NewsFeedNotification.NewsHoverOut, this);

				newsTitle.interactable = true;
				newsText.interactable = true;

				isFullScreenMode = true;
			}
		}
	}
	public void OnPointerEnter(PointerEventData ev){
		if(!isFullScreenMode)
			m_app.Notify(NewsFeedNotification.NewsHoverIn, this, newsTitle.text);
	}
	public void OnPointerExit(PointerEventData ev){
		if(!isFullScreenMode){
			if(!m_isClicking)
				m_app.Notify(NewsFeedNotification.NewsHoverOut, this);
		}
	}
	public void OnBeginDrag(PointerEventData ev){
		if(!isFullScreenMode){
			m_isDragging = true;

			gameObject.transform.SetAsLastSibling();
			Vector3 normalizedPosition = m_camera.ScreenToViewportPoint(new Vector3(ev.pressPosition.x, ev.pressPosition.y, 0));
			float offSetX = gameObject.transform.localPosition.x - (normalizedPosition.x*NewsFeedModel.SCREEN_WIDHT);
			float offSetY = gameObject.transform.localPosition.y - (normalizedPosition.y*NewsFeedModel.SCREEN_HEIGHT);

			gameObject.transform.rotation = Quaternion.identity;

			m_dragOffset = new Vector2(offSetX, offSetY);
		}
	}
	public void OnDrag(PointerEventData ev){
		if(!isFullScreenMode){
			if(Input.touchCount <= 1){
				//DRAG
				Vector3 normalizedPosition = m_camera.ScreenToViewportPoint(new Vector3(ev.position.x, ev.position.y, 0));
				Vector3 rPosition = new Vector3(normalizedPosition.x*NewsFeedModel.SCREEN_WIDHT+m_dragOffset.x, normalizedPosition.y*NewsFeedModel.SCREEN_HEIGHT+m_dragOffset.y, 0);

				if(normalizedPosition.x > 0f && normalizedPosition.x < 1f && normalizedPosition.y > 0f && normalizedPosition.y < 1f)
					gameObject.transform.localPosition = rPosition;
			}
		}
	}
	public void OnEndDrag(PointerEventData ev){
		m_isDragging = false;
	}
}
