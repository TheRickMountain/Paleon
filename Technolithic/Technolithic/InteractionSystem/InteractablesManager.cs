using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Technolithic
{
    public class InteractablesManager
    {
        private readonly Dictionary<InteractionType, List<Interactable>> interactionTypeInteractablesDict = new();

        //private readonly Dictionary<int, Dictionary<InteractionType, List<Interactable>>> interactionTypeEntitiesByRoom = new();

        public InteractablesManager(InteractionsDatabase interactionsDatabase)
        {
            foreach (InteractionData interactionData in interactionsDatabase.AllInteractions)
            {
                interactionTypeInteractablesDict.Add(interactionData.InteractionType, new List<Interactable>());
            }
        }

        public void AddInteractable(InteractionType interactionType, Interactable interactable)
        {
            interactionTypeInteractablesDict[interactionType].Add(interactable);

            //var uniqueRoomIds = entity.GetRoomIds().Distinct();

            //foreach (int roomId in uniqueRoomIds)
            //{
            //    AddToRoomCache(interactionTypeId, entity, roomId);
            //    SortEntitiesInRoom(interactionTypeId, roomId);
            //}

            //SortGlobalEntities(interactionTypeId);
        }

        public void RemoveInteractable(InteractionType interactionType, Interactable interactable)
        {
            interactionTypeInteractablesDict[interactionType].Remove(interactable);

            //var uniqueRoomIds = interactable.GetRoomIds().Distinct();

            //foreach (int roomId in uniqueRoomIds)
            //{
            //    if (roomId == -1) continue;

            //    if (interactionTypeEntitiesByRoom.TryGetValue(roomId, out var byType))
            //    {
            //        if (byType.TryGetValue(interactionType, out var list))
            //        {
            //            list.Remove(interactable);

            //            if (list.Count == 0)
            //                byType.Remove(interactionType);
            //        }

            //        if (byType.Count == 0)
            //            interactionTypeEntitiesByRoom.Remove(roomId);
            //    }
            //}
        }

        public Interactable GetFirstInteractable(int zoneId, InteractionType interactionType)
        {
            // TODO: It is necessary to return the building located in the required zone.

            //if (interactionTypeEntitiesByRoom.TryGetValue(roomId, out var byType))
            //{
            //    if (byType.TryGetValue(interactionTypeId, out var list) && list.Count > 0)
            //        return list[0];
            //}
            //return null;

            // TODO: temp

            if (interactionTypeInteractablesDict[interactionType].Count == 0) return null;

            return interactionTypeInteractablesDict[interactionType][0];
        }

        //public List<InteractableEntity> TryGetTopPriorityEntities(int roomId, InteractionTypeId interactionTypeId)
        //{
        //    // 1. Пытаемся получить список всех сущностей данного типа в данной комнате (зоне)
        //    if (interactionTypeEntitiesByRoom.TryGetValue(roomId, out var byType) &&
        //        byType.TryGetValue(interactionTypeId, out var entities) &&
        //        entities.Any())
        //    {
        //        // 2. Находим максимальное значение приоритета в этом списке.
        //        //    Поскольку список уже отсортирован, первый элемент всегда имеет наивысший приоритет.
        //        int topPriority = entities[0].Priority;

        //        // 3. Выбираем все сущности с этим приоритетом.
        //        //    Используем LINQ для фильтрации. TakeWhile - очень эффективен для отсортированных списков.
        //        return entities.TakeWhile(e => e.Priority == topPriority).ToList();
        //    }

        //    return null;
        //}

        //private void AddToRoomCache(InteractionTypeId interactionTypeId, InteractableEntity entity, int roomId)
        //{
        //    if (roomId == -1) return;

        //    if (!interactionTypeEntitiesByRoom.TryGetValue(roomId, out var byType))
        //    {
        //        byType = new Dictionary<InteractionTypeId, List<InteractableEntity>>();
        //        interactionTypeEntitiesByRoom[roomId] = byType;
        //    }

        //    if (!byType.TryGetValue(interactionTypeId, out var list))
        //    {
        //        list = new List<InteractableEntity>();
        //        byType[interactionTypeId] = list;
        //    }

        //    if (!list.Contains(entity))
        //    {
        //        list.Add(entity);
        //    }
        //}

        //public void RebuildEntitiesByRooms()
        //{
        //    interactionTypeEntitiesByRoom.Clear();

        //    foreach (var (typeId, entities) in interactionTypeEntitiesMap)
        //    {
        //        foreach (var entity in entities)
        //        {
        //            var uniqueRoomIds = entity.GetRoomIds().Distinct();
        //            foreach (var roomId in uniqueRoomIds)
        //            {
        //                AddToRoomCache(typeId, entity, roomId);
        //            }
        //        }
        //    }

        //    foreach (var room in interactionTypeEntitiesByRoom.Values)
        //    {
        //        foreach (var list in room.Values)
        //        {
        //            list.Sort((a, b) => b.Priority.CompareTo(a.Priority));
        //        }
        //    }
        //}

        //public void SortGlobalEntities(InteractionTypeId interactionTypeId)
        //{
        //    if (interactionTypeEntitiesMap.TryGetValue(interactionTypeId, out var globalList))
        //    {
        //        globalList.Sort((a, b) => b.Priority.CompareTo(a.Priority));
        //    }
        //}

        //public void SortEntitiesInRoom(InteractionTypeId interactionTypeId, int roomId)
        //{
        //    if (roomId == -1) return;

        //    if (interactionTypeEntitiesByRoom.TryGetValue(roomId, out var byType))
        //    {
        //        if (byType.TryGetValue(interactionTypeId, out var list))
        //        {
        //            list.Sort((a, b) => b.Priority.CompareTo(a.Priority));
        //        }
        //    }
        //}
    }
}
