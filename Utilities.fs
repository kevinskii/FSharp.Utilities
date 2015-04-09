/// Contains a set of extensions and other functions to support common tasks in F# data processing.
module FSharp.Utilities

/// <summary>
/// Similar to F#'s Seq.groupBy function, but removes the key from the grouped values. The main purpose
/// is to shorten grouped tuples to make them easier to work with and also to reduce memory overhead.
/// </summary>
/// Example:
/// <c>
/// // List of animals' species, names, and ages
/// let animals = [
///     ("Dog","Herbert",12);
///     ("Cat","Annie",3);
///     ("Dog","Lawrence",5);
///     ("Cat","Fred",6);]
///
/// // Group the animals by species
/// let animalsGroupedBySpecies = 
///     animals |> groupValuesBy (fun (species,name,age) -> species,(name,age)) |> Seq.toList
///
/// // Result of the above in F# Interactive (note how species is not  one of the grouped values)
/// // val it : (string * seq<string * int>) list =
/// //   [("Dog", seq [("Herbert", 12); ("Lawrence", 5)]);
/// //    ("Cat", seq [("Annie", 3); ("Fred", 6)])]
/// </c>
let groupValuesBy (projection:'T->('Key*'Values)) (source:#seq<'T>) =
    // Split the elements into (key,values) tuples
    Seq.map projection source
    // Group by key
    |> Seq.groupBy (fun (key,_) -> key)
    // Remove the key from the grouped values 
    |> Seq.map (fun (key,valsIncludingKey) -> key, valsIncludingKey |> Seq.map (fun (_,valsWithoutKey) -> valsWithoutKey))


/// <summary>
/// Performs a lazy groupBy of items that are known ahead of time to be clusted by key. Whereas the normal Seq.groupBy
/// must process all of the items in a potentially huge data set, this function is able to group one cluster at a time
/// in order to reduce memory overhead.
/// WARNING: Erroneous grouping will occur if the data set contains non-adjacent keys of the same value.
/// </summary>
/// Example:
/// <c>
/// // List of animals' species, names, and ages. We know ahead of time that these items are clustered by species,
/// // meaning that all animals of the same species are adjacent to one another
/// let clusteredAnimals = [
///     ("Dog","Herbert",12);
///     ("Dog","Lawrence",5);
///     ("Cat","Annie",3);
///     ("Cat","Fred",6);]
///     ("Cat","Tom",9);]
///     ("Horse","Fernando",16);]
///
/// // Group the animals by species, in the same way the Seq.groupBy would be used
/// let animalsGroupedBySpecies = 
///     clusteredAnimals |> groupClusteredBy (fun (species,_,_) -> species)
/// </c>
let groupClusteredBy (projection:'T->'Key) (source:#seq<'T>) =
    let prevKey = ref (projection (Seq.head source)) // Get a mutable key of the first item in the sequence
    let enumerator = source.GetEnumerator()
    let cluster = ResizeArray<'T>()

    // Enumerate through the source sequence and do a lazy groupBy of adjacent keys which have the same value
    seq {
        while enumerator.MoveNext() do
            // If this item's key doesn't match the previous ones', then return the group of items which had the same key
            let currItem = enumerator.Current
            let currKey = projection currItem         
            if currKey <> prevKey.Value then
                yield (prevKey.Value, cluster.ToArray() |> Seq.ofArray)
                prevKey := currKey
                cluster.Clear()
            cluster.Add currItem
        // Return the remaining cluster of values
        yield (prevKey.Value, cluster.ToArray() |> Seq.ofArray)
    }
