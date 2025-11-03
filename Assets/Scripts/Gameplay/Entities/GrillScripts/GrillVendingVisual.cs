using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Gameplay.LevelData;
using Manager;
using Sonat.Enums;
using SonatFramework.Scripts.Utils;
using SonatFramework.Systems;
using SonatFramework.Systems.ObjectPooling;
using TMPro;
using UnityEngine;

namespace Gameplay.Entities.GrillScripts
{
    public class GrillVendingVisual : GrillVisual
    {
        [SerializeField] private TMP_Text txtNumLayer;
        [SerializeField] private Transform container;
        [SerializeField] private Transform progress;

        private List<VendingGrillTileInProgress> tiles = new();

        private int maxNumLayer = 0;

        public override void SetDefaultGrill(GrillData grillData)
        {
            base.SetDefaultGrill(grillData);

            int numLayer = grillData.layer.Count;
            txtNumLayer.text = numLayer.ToString();
            progress.gameObject.SetActive(true);
            tiles.Clear();
            SpanwTiles(numLayer);

            maxNumLayer = numLayer;
        }

        private async UniTask SpanwTiles(int numLayer)
        {
            for (int i = 0; i < numLayer; i++)
            {
                var grill = GetComponentInParent<GrillBase>();
                var gameFactory = grill.GrillBaseBehaviorSO.gameFactorySO;
                var tile = await gameFactory.CreateItem<VendingGrillTileInProgress>("VendingGrillTileInProgress");
                tile.Setup(i, numLayer, container);
                tiles.Add(tile);
            }
        }
        protected override void SetVisual()
        {
            var grillBase = GetComponentInParent<GrillBase>();
            grillBase.GrillBaseBehaviorSO.grillVisualSO.SetGrillBaseVisual(this, stove, lid, stoveType, lidType);

        }

        // public override void UpdateSubGrill()
        // {
        //     if( numLayer <= 0) return;
        //     numLayer--;
        //     txtNumLayer.text = numLayer.ToString();
        //
        //     tiles[numLayer].transform.DOScale(0, 0.5f).SetEase(Ease.InBack).OnComplete(() =>
        //     {
        //         poolingService.Instance.ReturnObj(tiles[numLayer]);
        //         tiles.RemoveAt(numLayer);
        //     });
        //
        //     if (numLayer == 0)
        //     {
        //         primaryVendingGrill.SetLockItems(true);
        //         SonatUtils.DelayCall(0.5f, CloseGrill, this);
        //     }
        // }

        public void UpdateLayer(int numLayer)
        {
            txtNumLayer.text = numLayer.ToString();

            if (numLayer >= tiles.Count) return;
            tiles[numLayer].transform.DOScale(0, 0.5f).SetEase(Ease.InBack).OnComplete(() =>
            {
                var grill = GetComponentInParent<GrillBase>();
                var gameFactory = grill.GrillBaseBehaviorSO.gameFactorySO;
                gameFactory.ReturnEntity(tiles[numLayer]);
                tiles.RemoveAt(numLayer);
            });
        }

        public async UniTask SetLayer(int numLayer)
        {
            txtNumLayer.text = numLayer.ToString();
            while (numLayer - 1 >= tiles.Count)
            {
                var grill = GetComponentInParent<GrillBase>();
                var gameFactory = grill.GrillBaseBehaviorSO.gameFactorySO;
                var tile = await gameFactory.CreateItem<VendingGrillTileInProgress>("VendingGrillTileInProgress");
                tile.Setup(tiles.Count, maxNumLayer, container);
                tiles.Add(tile);
            }

            for (int i = 0; i < tiles.Count; i++)
            {
                if (i >= numLayer)
                    tiles[i].transform.DOScale(0, 0.5f).SetEase(Ease.InBack).OnComplete(() =>
                    {
                        var grill = GetComponentInParent<GrillBase>();
                        var gameFactory = grill.GrillBaseBehaviorSO.gameFactorySO;
                        gameFactory.ReturnEntity(tiles[numLayer]);
                        tiles.RemoveAt(numLayer);
                    });
            }
        }

        public override void OnReturnGrill()
        {
            base.OnReturnGrill();
            if (tiles != null)
            {
                foreach (var tile in tiles)
                {
                    var grill = GetComponentInParent<GrillBase>();
                    var gameFactory = grill.GrillBaseBehaviorSO.gameFactorySO;
                    gameFactory.ReturnEntity(tile);
                }
            }

            tiles.Clear();
        }

        public void Unlock()
        {
            progress.gameObject.SetActive(false);
        }
    }
}