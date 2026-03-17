using System;
using UnityEngine;
using UnityEngine.UI;

public class Journal : MonoBehaviour
{
    [SerializeField] private Button _journalButton;
    [SerializeField] private GameObject _fullJournal;
    [Space(20)]
    [SerializeField] private Transform _journalContent;
    [Space(20)]
    [SerializeField] private GameObject _simplePhrase;
    [SerializeField] private GameObject _space;

    public static bool IsJournalOpen {  get; private set; }

    private void OnEnable()
    {
        DialogueViewer.OnCreditBookAction += SetJournalButton_OnCreditBookAction;
        DialogueViewer.OnPhraseChanged += SaveSimplePhrase;
        DialogueViewer.OnDialogueEnded += CreateSpace;

        _journalButton.onClick.AddListener(OpenCloseJournal);

        IsJournalOpen = false;
        _fullJournal.SetActive(false);
    }

    private void OnDisable()
    {
        DialogueViewer.OnCreditBookAction -= SetJournalButton_OnCreditBookAction;
        DialogueViewer.OnPhraseChanged -= SaveSimplePhrase;
        DialogueViewer.OnDialogueEnded -= CreateSpace;

        _journalButton.onClick.RemoveListener(OpenCloseJournal);
    }

    private void SaveSimplePhrase(DialogueBaseClass phraseData)
    {
        JournalSimplePhrase currentSimplePhrase = Instantiate(_simplePhrase, _journalContent).GetComponent<JournalSimplePhrase>();
        currentSimplePhrase.Name.text = phraseData.simplePhrase.InputName;
        currentSimplePhrase.Text.text = phraseData.simplePhrase.InputText;
    }

    private void CreateSpace()
    {
        Instantiate(_space, _journalContent);
    }

    private void OpenCloseJournal()
    {
        _fullJournal.SetActive(!_fullJournal.activeSelf);
        IsJournalOpen = _fullJournal.activeSelf;
    }

    private void SetJournalButton_OnCreditBookAction(object sender, EventArgs e)
    {
        _journalButton.interactable = !_journalButton.IsInteractable();
    }
}
