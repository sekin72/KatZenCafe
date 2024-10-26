using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CerberusFramework.Core.Scenes;
using CerberusFramework.Managers.Vibration;
using TMPro;
using UnityEngine;
using VContainer;
using static TMPro.TMP_Dropdown;

namespace CFGameClient.CFDemoScene
{
    public class VibrationDemoPanel : DemoPanel
    {
        [SerializeField] private TMP_Dropdown _vibrationDropdown;
        private VibrationManager _vibrationManager;
        private List<VibrationType> _vibrationTypes;

        [Inject]
        public void Inject(VibrationManager vibrationManager)
        {
            _vibrationManager = vibrationManager;
        }

        public override void Initialize(SceneController sceneController, CancellationToken cancellationToken)
        {
            base.Initialize(sceneController, cancellationToken);

            _vibrationTypes = VibrationType.GetEnumerable().ToList();
            _vibrationTypes.RemoveAt(0);

            _vibrationDropdown.ClearOptions();
            foreach (var vibration in _vibrationTypes)
            {
                _vibrationDropdown.options.Add(new OptionData(vibration.Name));
            }

            _vibrationDropdown.SetValueWithoutNotify(0);
            _vibrationDropdown.RefreshShownValue();
        }

        public override void OnButtonClicked()
        {
            _vibrationManager.Vibrate(_vibrationTypes[_vibrationDropdown.value]);
        }
    }
}
