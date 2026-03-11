using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// SkillNodeUI is a node that reference to SkillNode class (Don't mix between the two).
/// With this SkillNode can reference to its parent and also children.
/// 
/// In default: 
/// parent SkillNodeUI does NOT have a parent Node (i.e. _skillNode.parentNode = null)
/// 
/// Leaves of the tree does NOT have any children. (i.e. _skillNode.children.Count = 0)
/// </summary>
public class SkillNodeUI : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private Image _skillBorder;

    /// <summary>
    /// _borderSprites[0]: Selected border
    /// _borderSprites[1]: Unselected learnt border
    /// _borderSprites[2]: Unselected unlocked border
    /// _borderSprites[3]: Unselected locked border
    /// </summary>
    [SerializeField]
    private Sprite[] _borderSprites;

    [SerializeField] 
    private SkillNode _skillNode;

    [SerializeField]
    private List<Image> _upgradeLinkImages;

    /// <summary>
    /// _linkColor[0]: grey,
    /// _linkColor[1]: white,
    /// _linkColor[2]: yellow.
    /// </summary>
    [SerializeField]
    private Color[] _linkColor;

    [SerializeField]
    private Image skillIcon;

    private SkillNodeSelector _parentSelector;

    public SkillNode SkillNode => _skillNode;
    // Start is called before the first frame update
    private void Start()
    {
        if(_skillNode.SkillData == null)
        {
            Debug.LogError("Skill Data cannot be null.");
            return;
        }

        skillIcon.sprite = _skillNode.SkillData.UpgradeSprite;

        if(!_skillNode.SkillData.Unlockable)
        {
            for (int i = 0; i < _upgradeLinkImages.Count; i++)
            {
                _upgradeLinkImages[i].color = _linkColor[0];
            }
        }
        else if(_skillNode.SkillData.IsLocked)
        {
            for (int i = 0; i < _upgradeLinkImages.Count; i++)
            {
                _upgradeLinkImages[i].color = _linkColor[1];
            }
        }
        else
        {
            for (int i = 0; i < _upgradeLinkImages.Count; i++)
            {
                _upgradeLinkImages[i].color = _linkColor[2];
            }
        }

        _parentSelector = transform.parent.GetComponent<SkillNodeSelector>();
    }

    public void Upgrade()
    {
        if (_skillNode.SkillData.Unlockable) { 
            _skillNode.ApplyUpgrade();
            SkillTreeManager.OnSkillUpgrade.Invoke(this);
            for (int i = 0; i < _upgradeLinkImages.Count; i++)
            {
                _upgradeLinkImages[i].color = _linkColor[2];
            }
        }
    }
    public void UpdateUpgrade()
    {
        if (_skillNode.SkillData.IsLocked && _skillNode.SkillData.Unlockable) {
            _skillBorder.sprite = _borderSprites[2];
            for (int i = 0; i < _upgradeLinkImages.Count; i++)
            {
                _upgradeLinkImages[i].color = _linkColor[1];
            }
        }
        else _skillBorder.sprite = _borderSprites[3];
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Upgrade();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ToolTipSystem.Instance.OnToolTipSelected(this);
        _parentSelector?.UIHover(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ToolTipSystem.Instance.CloseToolTip();
    }

    public void Highlight()
    {
        _skillBorder.sprite = _borderSprites[0];
    }

    public void UnHighlight()
    {
        if (!_skillNode.SkillData.Unlockable)
        {
            _skillBorder.sprite = _borderSprites[3];
        }
        else if (_skillNode.SkillData.IsLocked)
        {
            _skillBorder.sprite = _borderSprites[2];
        }
        else
        {
            _skillBorder.sprite = _borderSprites[1];
        }
    }
}
