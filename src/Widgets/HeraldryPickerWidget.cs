using BattleTech;
using BattleTech.Data;
using BattleTech.UI;
using HeraldryPicker.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace HeraldryPicker.Widgets
{
    /// <summary>
    /// A widget for selecting a heraldry (faction paint scheme) from a scrollable list
    /// </summary>
    public class HeraldryPickerWidget : MonoBehaviour
    {
        public RectTransform listParent;
        public GameObject loadingNotification;
        private DataManager dataManager;
        private UnityAction<HeraldryDef> heraldrySelectedCB;
        private List<HeraldryDef> allHeraldryDefs = [];
        private List<HeraldryPickerElement> allHeraldryElements = [];
        private HeraldryDef selectedHeraldry;
        private HeraldryPickerElement selectedElement;
        private string initialSelectedHeraldryID;

        public string SelectedHeraldryId => selectedHeraldry?.Description?.Id ?? "";

        public void SetData(DataManager dataManager, string selectedHeraldryId, UnityAction<HeraldryDef> heraldrySelectedCB)
        {
            this.dataManager = dataManager;
            this.heraldrySelectedCB = heraldrySelectedCB;
            this.initialSelectedHeraldryID = selectedHeraldryId;
            HeraldryExporter.ExportHeraldries(dataManager);
            StartCoroutine(InitHeraldryList());
        }

        private System.Collections.IEnumerator InitHeraldryList()
        {
            yield return null;
            PopulateHeraldryList(OnHeraldryLoadSuccess);
        }

        private void OnHeraldryLoadSuccess()
        {
            if (!string.IsNullOrEmpty(initialSelectedHeraldryID))
            {
                SelectHeraldryById(initialSelectedHeraldryID);
            }
        }

        private void PopulateHeraldryList(Action onSuccess)
        {
            ClearHeraldryList();
            loadingNotification?.SetActive(true);
            foreach (var kvp in dataManager.Heraldries)
            {
                var def = kvp.Value;
                if (def != null)
                {
                    allHeraldryDefs.Add(def);
                }
            }
            allHeraldryDefs = [.. allHeraldryDefs
                .OrderBy(def => FactionGroupManager.GetGroupForHeraldry(def.Description.Id.Replace("heraldrydef_", "")))
                .ThenBy(def => def.Description.Name)
                .ThenBy(def => def.Description.Id, new Utils.NaturalStringComparer())];
            loadingNotification?.SetActive(false);
            OnAllHeraldryLoaded();
            onSuccess?.Invoke();
        }

        private void OnAllHeraldryLoaded()
        {
            foreach (var def in allHeraldryDefs)
            {
                var element = CreateHeraldryElement(def);
                if (element != null)
                {
                    element.transform.SetParent(listParent, false);
                    allHeraldryElements.Add(element);
                }
            }
        }

        private void ClearHeraldryList()
        {
            foreach (var element in allHeraldryElements)
            {
                Destroy(element.gameObject);
            }
            allHeraldryElements.Clear();
            allHeraldryDefs.Clear();
        }

        private HeraldryPickerElement CreateHeraldryElement(HeraldryDef def)
        {
            var go = dataManager.PooledInstantiate("uixPrfPanl_crestItem-Element", BattleTechResourceType.UIModulePrefabs, null, null, null);
            if (go == null) return null;

            var element = go.GetComponent<HeraldryPickerElement>() ?? go.AddComponent<HeraldryPickerElement>();
            element.SetData(def, OnHeraldrySelected);
            return element;
        }

        private void OnHeraldrySelected(HeraldryDef def, bool assignCrest)
        {
            selectedElement?.SetSelected(false);
            selectedElement = allHeraldryElements.FirstOrDefault(e => e.heraldryDef == def);
            selectedElement?.SetSelected(true);

            selectedHeraldry = def;
            heraldrySelectedCB?.Invoke(def);

            var panel = GetComponentInParent<HeraldryCreatorPanel>();
            if (panel != null)
            {
                if (panel.activeDef != null)
                {
                    panel.activeDef.primaryMechColorID = def.primaryMechColorID;
                    panel.activeDef.secondaryMechColorID = def.secondaryMechColorID;
                    panel.activeDef.tertiaryMechColorID = def.tertiaryMechColorID;

                    if (assignCrest)
                    {
                        panel.activeDef.textureLogoID = def.textureLogoID;
                        panel.crestPicker.selectedCrest?.SetSelectedState(isSelected: false);
                        panel.crestPicker.selectedCrest = null;
                    }
                }

                panel.colorPicker?.SetData(def.primaryMechColorID, def.secondaryMechColorID, def.tertiaryMechColorID, new UnityAction(panel.ColorPickerRefresh));
                panel.RefreshHeraldry();
            }
        }

        public void SelectHeraldryById(string id)
        {
            foreach (var def in allHeraldryDefs)
            {
                if (def.Description.Id == id)
                {
                    selectedHeraldry = def;
                    heraldrySelectedCB?.Invoke(def);
                    break;
                }
            }
        }

        public void ResetSelection()
        {
            selectedElement?.SetSelected(false);
            selectedElement = null;
        }
    }
}