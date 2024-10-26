using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CerberusFramework.Core.Scenes;
using CerberusFramework.Managers.Sound;
using TMPro;
using UnityEngine;
using VContainer;
using static TMPro.TMP_Dropdown;

namespace CFGameClient.CFDemoScene
{
    public class SoundDemoPanel : DemoPanel
    {
        [SerializeField] private TMP_Dropdown _soundDropdown;
        private SoundManager _soundManager;
        private List<CFSoundTypes> _soundTypes;

        [Inject]
        public void Inject(SoundManager soundManager)
        {
            _soundManager = soundManager;
        }

        public override void Initialize(SceneController sceneController, CancellationToken cancellationToken)
        {
            base.Initialize(sceneController, cancellationToken);

            _soundTypes = CFSoundTypes.GetEnumerable().ToList();
            _soundTypes.RemoveAt(0);

            _soundDropdown.ClearOptions();
            foreach (var sound in _soundTypes)
            {
                _soundDropdown.options.Add(new OptionData(sound.Name));
            }

            _soundDropdown.SetValueWithoutNotify(0);
            _soundDropdown.RefreshShownValue();
        }

        public override void OnButtonClicked()
        {
            _soundManager.PlayOneShot(_soundTypes[_soundDropdown.value]);
        }
    }
}
