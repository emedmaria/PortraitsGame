using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Portraits
{
	[RequireComponent(typeof(BoxCollider))]
	public class PortraitFrame : MonoBehaviour
	{
		private const float VANISH_TS = .6f;

		public Action<int, Vector2> OnSelection;

		//To identify Pair matches
		public int PortraitID{ get; set;}
		//Position i,j in the grid
		public Vector2 Coordinates{ get; set; }

		//To know whether portrait is visible
		private bool _characterVis = true;
		public bool CharacterVisible
		{
			get { return _characterVis; }
		}

		//To avoid interaction
		public bool Enabled { get; set;}

		//Cache component needed
		private Transform _pTransform = null;
		private SpriteRenderer _srCharacter = null;

		//Settings
		private Vector3 _initialScale = Vector3.one;
		private Vector3 _endScale = new Vector3(2.0f, 2.2f, 0f);
		private float _timeScale = 0.1f;

		#region MonoBehaviour methods
		void Awake()
		{
			_pTransform = transform;
			Transform characterGo = _pTransform.Find("Character");
			_srCharacter = characterGo.GetComponent<SpriteRenderer>();
			_initialScale = _pTransform.localScale;
			//Make sure the collider acts like a trigger
			BoxCollider bCollider = _pTransform.GetComponent<BoxCollider>();
			bCollider.isTrigger = true;
		}

		void OnMouseDown()
		{
			if (!Enabled)
				return;
			SoundManager.instance.PlaySFXById(SoundManager.SFX_SELECTION);
			//Show character  until second click ?
			ShowSolution(true);

			if (OnSelection != null)
				OnSelection(PortraitID, Coordinates);
			
		}

		void OnMouseOver()
		{
			if (!Enabled)
				return;

			//TODO: Lerping the scale
			//StartCoroutine(LerpScaleUp());
			_pTransform.localScale = _endScale;
		}

		void OnMouseExit()
		{
			if (!Enabled)
				return;
			//TODO: Lerping the scale
			//StartCoroutine(LerpScaleDown());
			_pTransform.localScale = _initialScale;
		}
		#region 
		/// <summary>
		/// Hides or Shows the content of the frame (Character)
		/// </summary>
		public void ShowSolution(bool show)
		{
			_srCharacter.enabled = show;
		}

		public IEnumerator DoVanish(float time = VANISH_TS)
		{
			yield return new WaitForSeconds(time);
			Visible(false);
		}

		public IEnumerator DoFlip(bool flip , float time = VANISH_TS)
		{
			yield return new WaitForSeconds(time);
			ShowSolution(!flip);
		}

		public void Visible(bool value)
		{
			_pTransform.gameObject.SetActive(value);
		}

		public void DestroyG0()
		{
			GameObject.Destroy(this.gameObject);
		}
		#endregion

		#endregion
		#region Utilites
		IEnumerator LerpScaleUp()
		{
			float progress = 0;

			while (progress <= 1)
			{
				_pTransform.localScale = Vector3.Lerp(_pTransform.localScale, _endScale, progress);
				progress += Time.deltaTime * _timeScale;
				yield return null;
			}
			_pTransform.localScale = _endScale;

		}

		IEnumerator LerpScaleDown()
		{
			float progress = 0;

			while (progress <= 1)
			{
				_pTransform.localScale = Vector3.Lerp(_pTransform.localScale, _initialScale, progress);
				progress += Time.deltaTime * _timeScale;
				yield return null;
			}
			_pTransform.localScale = _initialScale;

		}
		#endregion
	}

}

