// ts2fable 0.7.0
module rec Vis
open System
open Fable.Core
open Browser.Types

type Array<'T> = System.Collections.Generic.IList<'T>

// type MomentInput = Moment.MomentInput
// type MomentFormatSpecification = Moment.MomentFormatSpecification
// type Moment = Moment.Moment

type [<AllowNullLiteral>] IExports =
    abstract DataSet: DataSetStatic
    abstract DataView: DataViewStatic
    abstract Graph2d: Graph2dStatic
    abstract Timeline: TimelineStatic
    // abstract Timeline: TimelineStaticStatic
    abstract Network: NetworkStatic

// type [<AllowNullLiteral>] MomentConstructor1 =
//     [<Emit "$0($1...)">] abstract Invoke: ?inp: MomentInput * ?format: MomentFormatSpecification * ?strict: bool -> Moment

// type [<AllowNullLiteral>] MomentConstructor2 =
//     [<Emit "$0($1...)">] abstract Invoke: ?inp: MomentInput * ?format: MomentFormatSpecification * ?language: string * ?strict: bool -> Moment

// type MomentConstructor =
//     U2<MomentConstructor1, MomentConstructor2>

type IdType =
    U2<string, float>

type SubgroupType =
    IdType

type DateType =
    U3<DateTime, float, string>

type [<StringEnum>] [<RequireQualifiedAccess>] DirectionType =
    | From
    | To

type HeightWidthType =
    IdType

type [<StringEnum>] [<RequireQualifiedAccess>] TimelineItemType =
    | Box
    | Point
    | Range
    | Background

type [<StringEnum>] [<RequireQualifiedAccess>] TimelineAlignType =
    | Auto
    | Center
    | Left
    | Right

type [<StringEnum>] [<RequireQualifiedAccess>] TimelineTimeAxisScaleType =
    | Millisecond
    | Second
    | Minute
    | Hour
    | Weekday
    | Day
    | Week
    | Month
    | Year

type [<StringEnum>] [<RequireQualifiedAccess>] TimelineEventPropertiesResultWhatType =
    | Item
    | Background
    | Axis
    | [<CompiledName "group-label">] GroupLabel
    | [<CompiledName "custom-time">] CustomTime
    | [<CompiledName "current-time">] CurrentTime

type [<StringEnum>] [<RequireQualifiedAccess>] TimelineEvents =
    | CurrentTimeTick
    | Click
    | Contextmenu
    | DoubleClick
    | Drop
    | MouseOver
    | MouseDown
    | MouseUp
    | MouseMove
    | GroupDragged
    | Changed
    | Rangechange
    | Rangechanged
    | Select
    | Itemover
    | Itemout
    | Timechange
    | Timechanged

type [<StringEnum>] [<RequireQualifiedAccess>] Graph2dStyleType =
    | Line
    | Bar
    | Points

type [<StringEnum>] [<RequireQualifiedAccess>] Graph2dBarChartAlign =
    | Left
    | Center
    | Right

type [<StringEnum>] [<RequireQualifiedAccess>] Graph2dDrawPointsStyle =
    | Square
    | Circle

type [<StringEnum>] [<RequireQualifiedAccess>] LegendPositionType =
    | [<CompiledName "top-right">] TopRight
    | [<CompiledName "top-left">] TopLeft
    | [<CompiledName "bottom-right">] BottomRight
    | [<CompiledName "bottom-left">] BottomLeft

type [<StringEnum>] [<RequireQualifiedAccess>] ParametrizationInterpolationType =
    | Centripetal
    | Chordal
    | Uniform
    | Disabled

type [<StringEnum>] [<RequireQualifiedAccess>] TopBottomEnumType =
    | Top
    | Bottom

type [<StringEnum>] [<RequireQualifiedAccess>] RightLeftEnumType =
    | Right
    | Left

type [<AllowNullLiteral>] LegendPositionOptions =
    abstract visible: bool option with get, set
    abstract position: LegendPositionType option with get, set

type [<AllowNullLiteral>] LegendOptions =
    abstract enabled: bool option with get, set
    abstract icons: bool option with get, set
    abstract iconSize: float option with get, set
    abstract iconSpacing: float option with get, set
    abstract left: LegendPositionOptions option with get, set
    abstract right: LegendPositionOptions option with get, set

type [<AllowNullLiteral>] DataItem =
    abstract className: string option with get, set
    abstract content: string with get, set
    abstract ``end``: DateType option with get, set
    abstract group: obj option with get, set
    abstract id: IdType option with get, set
    abstract start: DateType with get, set
    abstract style: string option with get, set
    abstract subgroup: SubgroupType option with get, set
    abstract title: string option with get, set
    abstract ``type``: string option with get, set
    abstract editable: bool option with get, set

type [<AllowNullLiteral>] PointItem =
    inherit DataItem
    abstract x: string with get, set
    abstract y: float with get, set

type [<AllowNullLiteral>] SubGroupStackOptions =
    [<Emit "$0[$1]{{=$2}}">] abstract Item: name: string -> bool with get, set

type [<AllowNullLiteral>] DataGroup =
    abstract className: string option with get, set
    abstract content: string with get, set
    abstract id: IdType with get, set
    abstract options: DataGroupOptions option with get, set
    abstract style: string option with get, set
    abstract subgroupOrder: U2<string, (unit -> unit)> option with get, set
    abstract title: string option with get, set
    abstract nestedGroups: ResizeArray<IdType> option with get, set
    abstract subgroupStack: U2<SubGroupStackOptions, bool> option with get, set
    abstract visible: bool option with get, set
    abstract showNested: bool option with get, set

type [<AllowNullLiteral>] DataGroupOptions =
    abstract drawPoints: U2<Graph2dDrawPointsOption, (unit -> unit)> option with get, set
    abstract excludeFromLegend: bool option with get, set
    abstract interpolation: U2<bool, InterpolationOptions> option with get, set
    abstract shaded: Graph2dShadedOption option with get, set
    abstract style: string option with get, set
    abstract yAxisOrientation: RightLeftEnumType option with get, set

type [<AllowNullLiteral>] InterpolationOptions =
    abstract parametrization: ParametrizationInterpolationType with get, set

type [<AllowNullLiteral>] TimelineEditableOption =
    abstract add: bool option with get, set
    abstract remove: bool option with get, set
    abstract updateGroup: bool option with get, set
    abstract updateTime: bool option with get, set
    abstract overrideItems: bool option with get, set

type [<AllowNullLiteral>] TimelineFormatLabelsFunction =
    [<Emit "$0($1...)">] abstract Invoke: date: DateTime * scale: string * step: float -> string

type [<AllowNullLiteral>] TimelineFormatLabelsOption =
    abstract millisecond: string option with get, set
    abstract second: string option with get, set
    abstract minute: string option with get, set
    abstract hour: string option with get, set
    abstract weekday: string option with get, set
    abstract day: string option with get, set
    abstract week: string option with get, set
    abstract month: string option with get, set
    abstract year: string option with get, set

type [<AllowNullLiteral>] TimelineFormatOption =
    abstract minorLabels: U2<TimelineFormatLabelsOption, TimelineFormatLabelsFunction> option with get, set
    abstract majorLabels: U2<TimelineFormatLabelsOption, TimelineFormatLabelsFunction> option with get, set

type [<AllowNullLiteral>] TimelineGroupEditableOption =
    abstract add: bool option with get, set
    abstract remove: bool option with get, set
    abstract order: bool option with get, set

type [<AllowNullLiteral>] TimelineHiddenDateOption =
    abstract start: DateType with get, set
    abstract ``end``: DateType with get, set
    abstract repeat: TimelineHiddenDateOptionRepeat option with get, set

type [<AllowNullLiteral>] TimelineItemsAlwaysDraggableOption =
    abstract item: bool option with get, set
    abstract range: bool option with get, set

type [<AllowNullLiteral>] TimelineMarginItem =
    abstract horizontal: float option with get, set
    abstract vertical: float option with get, set

type TimelineMarginItemType =
    U2<float, TimelineMarginItem>

type [<AllowNullLiteral>] TimelineMarginOption =
    abstract axis: float option with get, set
    abstract item: TimelineMarginItemType option with get, set

type [<AllowNullLiteral>] TimelineOrientationOption =
    abstract axis: string option with get, set
    abstract item: string option with get, set

type [<AllowNullLiteral>] TimelineTimeAxisOption =
    abstract scale: TimelineTimeAxisScaleType option with get, set
    abstract step: float option with get, set

type [<AllowNullLiteral>] TimelineRollingModeOption =
    abstract follow: bool option with get, set
    abstract offset: float option with get, set

type [<AllowNullLiteral>] TimelineTooltipOption =
    abstract followMouse: bool option with get, set
    abstract overflowMethod: TimelineTooltipOptionOverflowMethod option with get, set

type [<AllowNullLiteral>] TimelineOptionsConfigureFunction =
    [<Emit "$0($1...)">] abstract Invoke: option: string * path: ResizeArray<string> -> bool

type TimelineOptionsConfigureType =
    U2<bool, TimelineOptionsConfigureFunction>

type TimelineOptionsDataAttributesType =
    U3<bool, string, ResizeArray<string>>

type TimelineOptionsEditableType =
    U2<bool, TimelineEditableOption>

type [<AllowNullLiteral>] TimelineOptionsItemCallbackFunction =
    [<Emit "$0($1...)">] abstract Invoke: item: TimelineItem * callback: (TimelineItem option -> unit) -> unit

type [<AllowNullLiteral>] TimelineOptionsGroupCallbackFunction =
    [<Emit "$0($1...)">] abstract Invoke: group: TimelineGroup * callback: (TimelineGroup option -> unit) -> unit

type TimelineOptionsGroupEditableType =
    U2<bool, TimelineGroupEditableOption>

type TimelineOptionsGroupOrderType =
    U2<string, TimelineOptionsComparisonFunction>

type [<AllowNullLiteral>] TimelineOptionsGroupOrderSwapFunction =
    [<Emit "$0($1...)">] abstract Invoke: fromGroup: obj option * toGroup: obj option * groups: DataSet<DataGroup> -> unit

type TimelineOptionsHiddenDatesType =
    U2<TimelineHiddenDateOption, ResizeArray<TimelineHiddenDateOption>>

type TimelineOptionsItemsAlwaysDraggableType =
    U2<bool, TimelineItemsAlwaysDraggableOption>

type TimelineOptionsMarginType =
    U2<float, TimelineMarginOption>

type TimelineOptionsOrientationType =
    U2<string, TimelineOrientationOption>

type [<AllowNullLiteral>] TimelineOptionsSnapFunction =
    [<Emit "$0($1...)">] abstract Invoke: date: DateTime * scale: string * step: float -> U2<DateTime, float>

type [<AllowNullLiteral>] TimelineOptionsTemplateFunction =
    [<Emit "$0($1...)">] abstract Invoke: ?item: obj * ?element: obj * ?data: obj -> string

type [<AllowNullLiteral>] TimelineOptionsComparisonFunction =
    [<Emit "$0($1...)">] abstract Invoke: a: obj option * b: obj option -> float

type [<AllowNullLiteral>] TimelineOptions =
    abstract align: TimelineAlignType option with get, set
    abstract autoResize: bool option with get, set
    abstract clickToUse: bool option with get, set
    abstract configure: TimelineOptionsConfigureType option with get, set
    abstract dataAttributes: TimelineOptionsDataAttributesType option with get, set
    abstract editable: TimelineOptionsEditableType option with get, set
    abstract ``end``: DateType option with get, set
    abstract format: TimelineFormatOption option with get, set
    abstract groupEditable: TimelineOptionsGroupEditableType option with get, set
    abstract groupOrder: TimelineOptionsGroupOrderType option with get, set
    abstract groupOrderSwap: TimelineOptionsGroupOrderSwapFunction option with get, set
    abstract groupTemplate: TimelineOptionsTemplateFunction option with get, set
    abstract height: HeightWidthType option with get, set
    abstract hiddenDates: TimelineOptionsHiddenDatesType option with get, set
    abstract horizontalScroll: bool option with get, set
    abstract itemsAlwaysDraggable: TimelineOptionsItemsAlwaysDraggableType option with get, set
    abstract locale: string option with get, set
    abstract locales: obj option with get, set
    // abstract moment: MomentConstructor option with get, set
    abstract margin: TimelineOptionsMarginType option with get, set
    abstract max: DateType option with get, set
    abstract maxHeight: HeightWidthType option with get, set
    abstract maxMinorChars: float option with get, set
    abstract min: DateType option with get, set
    abstract minHeight: HeightWidthType option with get, set
    abstract moveable: bool option with get, set
    abstract multiselect: bool option with get, set
    abstract multiselectPerGroup: bool option with get, set
    abstract onAdd: TimelineOptionsItemCallbackFunction option with get, set
    abstract onAddGroup: TimelineOptionsGroupCallbackFunction option with get, set
    abstract onInitialDrawComplete: (unit -> unit) option with get, set
    abstract onUpdate: TimelineOptionsItemCallbackFunction option with get, set
    abstract onMove: TimelineOptionsItemCallbackFunction option with get, set
    abstract onMoveGroup: TimelineOptionsGroupCallbackFunction option with get, set
    abstract onMoving: TimelineOptionsItemCallbackFunction option with get, set
    abstract onRemove: TimelineOptionsItemCallbackFunction option with get, set
    abstract onRemoveGroup: TimelineOptionsGroupCallbackFunction option with get, set
    abstract order: TimelineOptionsComparisonFunction option with get, set
    abstract orientation: TimelineOptionsOrientationType option with get, set
    abstract rollingMode: TimelineRollingModeOption option with get, set
    abstract rtl: bool option with get, set
    abstract selectable: bool option with get, set
    abstract showCurrentTime: bool option with get, set
    abstract showMajorLabels: bool option with get, set
    abstract showMinorLabels: bool option with get, set
    abstract showTooltips: bool option with get, set
    abstract stack: bool option with get, set
    abstract stackSubgroups: bool option with get, set
    abstract snap: TimelineOptionsSnapFunction option with get, set
    abstract start: DateType option with get, set
    abstract template: TimelineOptionsTemplateFunction option with get, set
    abstract visibleFrameTemplate: TimelineOptionsTemplateFunction option with get, set
    abstract throttleRedraw: float option with get, set
    abstract timeAxis: TimelineTimeAxisOption option with get, set
    abstract ``type``: string option with get, set
    abstract tooltip: TimelineTooltipOption option with get, set
    abstract tooltipOnItemUpdateTime: U2<bool, TimelineOptionsTooltipOnItemUpdateTime> option with get, set
    abstract verticalScroll: bool option with get, set
    abstract width: HeightWidthType option with get, set
    abstract zoomable: bool option with get, set
    abstract zoomKey: string option with get, set
    abstract zoomMax: float option with get, set
    abstract zoomMin: float option with get, set

type TimelineAnimationType =
    U2<bool, AnimationOptions>

type [<AllowNullLiteral>] TimelineAnimationOptions =
    abstract animation: TimelineAnimationType option with get, set

type [<AllowNullLiteral>] TimelineEventPropertiesResult =
    /// The id of the clicked group
    abstract group: float option with get, set
    /// The id of the clicked item.
    abstract item: IdType option with get, set
    /// Absolute horizontal position of the click event.
    abstract pageX: float with get, set
    /// Absolute vertical position of the click event.
    abstract pageY: float with get, set
    /// Relative horizontal position of the click event.
    abstract x: float with get, set
    /// Relative vertical position of the click event.
    abstract y: float with get, set
    /// Date of the clicked event.
    abstract time: DateTime with get, set
    /// Date of the clicked event, snapped to a nice value.
    abstract snappedTime: DateTime with get, set
    /// Name of the clicked thing.
    abstract what: TimelineEventPropertiesResultWhatType option with get, set
    /// The original click event.
    abstract ``event``: Event with get, set

/// Options that can be passed to a DataSet.
type [<AllowNullLiteral>] DataSetOptions =
    inherit DataSetQueueOptions
    /// The name of the field containing the id of the items.
    /// When data is fetched from a server which uses some specific field to identify items,
    /// this field name can be specified in the DataSet using the option fieldId.
    /// For example CouchDB uses the field "_id" to identify documents.
    abstract fieldId: string option with get, set
    /// An object containing field names as key, and data types as value.
    /// By default, the type of the properties of items are left unchanged.
    /// Item properties can be normalized by specifying a field type.
    /// This is useful for example to automatically convert stringified dates coming
    /// from a server into JavaScript Date objects.
    /// The available data types are listed in section Data Types.
    abstract ``type``: obj option with get, set

type [<AllowNullLiteral>] DataSetQueueOptions =
    /// Queue data changes ('add', 'update', 'remove') and flush them at once.
    /// The queue can be flushed manually by calling DataSet.flush(),
    /// or can be flushed after a configured delay or maximum number of entries.
    /// When queue is true, a queue is created with default options.
    /// Options can be specified by providing an object:
    /// delay: number - The queue will be flushed automatically after an inactivity of this delay in milliseconds. Default value is null.
    /// Default value is null.
    /// max: number - When the queue exceeds the given maximum number of entries, the queue is flushed automatically. Default value is Infinity.
    /// Default value is Infinity.
    abstract queue: U2<obj option, bool> option with get, set

type [<AllowNullLiteral>] DataSet<'T> =
    /// The number of items in the DataSet.
    abstract length: float with get, set
    /// <summary>Add one or multiple items to the DataSet.
    /// Adding an item will fail when there already is an item with the same id.</summary>
    /// <param name="data">data can be a single item or an array with items.</param>
    /// <param name="senderId">Optional sender id.</param>
    abstract add: data: U2<'T, ResizeArray<'T>> * ?senderId: IdType -> ResizeArray<IdType>
    /// <summary>Clear all data from the DataSet.</summary>
    /// <param name="senderId">Optional sender id.</param>
    abstract clear: ?senderId: IdType -> ResizeArray<IdType>
    /// <summary>Find all distinct values of a specified field.
    /// If data items do not contain the specified field are ignored.</summary>
    /// <param name="field">The search term.</param>
    abstract distinct: field: string -> ResizeArray<obj option>
    /// Flush queued changes.
    /// Only available when the DataSet is configured with the option queue.
    abstract flush: unit -> unit
    /// <summary>Execute a callback function for every item in the dataset.</summary>
    /// <param name="callback">The item callback.</param>
    /// <param name="options">Optional options</param>
    abstract forEach: callback: ('T -> IdType -> unit) * ?options: DataSelectionOptions<'T> -> unit
    /// <summary>Get all items from the DataSet.</summary>
    /// <param name="options">Optional options.</param>
    abstract get: ?options: DataSelectionOptions<'T> -> ResizeArray<'T>
    /// <summary>Get a single item from the DataSet.</summary>
    /// <param name="id">The item id.</param>
    abstract get: id: IdType * ?options: DataSelectionOptions<'T> -> 'T option
    /// <summary>Get multiple items from the DataSet.</summary>
    /// <param name="ids">Array of item ids.</param>
    /// <param name="options">Optional options.</param>
    abstract get: ids: ResizeArray<IdType> * ?options: DataSelectionOptions<'T> -> ResizeArray<'T>
    /// Get the DataSet itself.
    /// In case of a DataView, this function does not return the DataSet
    /// to which the DataView is connected.
    abstract getDataSet: unit -> DataSet<'T>
    /// Get ids of all items or of a filtered set of items.
    abstract getIds: ?options: DataSelectionOptions<'T> -> ResizeArray<IdType>
    /// <summary>Map every item in the DataSet.</summary>
    /// <param name="callback">The mapping callback.</param>
    /// <param name="options">Optional options.</param>
    abstract map: callback: ('T -> IdType -> 'M) * ?options: DataSelectionOptions<'T> -> ResizeArray<'M>
    /// Find the item with maximum value of specified field.
    abstract max: field: string -> 'T
    /// Find the item with minimum value of specified field.
    abstract min: field: string -> 'T
    /// <summary>Subscribe from an event.</summary>
    /// <param name="event">The event name.</param>
    /// <param name="callback">a callback function which will be called each time the event occurs.</param>
    abstract on: ``event``: string * callback: (string -> obj option -> IdType -> unit) -> unit
    /// <summary>Unsubscribe to an event.</summary>
    /// <param name="event">The event name.</param>
    /// <param name="callback">The exact same callback that was used when calling 'on'.</param>
    abstract off: ``event``: string * callback: (string -> obj option -> IdType -> unit) -> unit
    /// <summary>Remove one or more items by id.</summary>
    /// <param name="id">The item id.</param>
    /// <param name="senderId">The sender id.</param>
    abstract remove: id: U2<IdType, ResizeArray<IdType>> * ?senderId: IdType -> ResizeArray<IdType>
    /// Set options for the DataSet.
    abstract setOptions: ?options: DataSetQueueOptions -> unit
    /// <summary>Update one or multiple existing items.
    /// When an item doesn't exist, it will be created.</summary>
    /// <param name="data">a single item or an array with items.</param>
    abstract update: data: U2<'T, ResizeArray<'T>> * ?senderId: IdType -> ResizeArray<IdType>

type [<AllowNullLiteral>] DataSetStatic =
    /// <summary>Creates an instance of DataSet.</summary>
    /// <param name="options">DataSet options.</param>
    [<Emit "new $0($1...)">] abstract Create: options: DataSetOptions -> DataSet<'T>
    /// <summary>Creates an instance of DataSet.</summary>
    /// <param name="data">An Array with items.</param>
    /// <param name="options">DataSet options.</param>
    [<Emit "new $0($1...)">] abstract Create: ?data: ResizeArray<'T> * ?options: DataSetOptions -> DataSet<'T>

/// The DataSet contains functionality to format, filter, and sort data retrieved
/// via the methods get, getIds, forEach, and map.
/// These methods can have these options as a parameter.
type [<AllowNullLiteral>] DataSelectionOptions<'T> =
    /// An array with field names, or an object with current field name
    /// and new field name that the field is returned as.
    /// By default, all properties of the items are emitted.
    /// When fields is defined, only the properties whose name is specified
    /// in fields will be included in the returned items.
    abstract fields: U2<ResizeArray<string>, obj option> option with get, set
    /// An object containing field names as key, and data types as value.
    /// By default, the type of the properties of an item are left unchanged.
    /// When a field type is specified, this field in the items will be converted to the specified type.
    /// This can be used for example to convert ISO strings containing a date to a JavaScript Date object,
    /// or convert strings to numbers or vice versa. The available data types are listed in section Data Types.
    abstract ``type``: obj option with get, set
    /// Items can be filtered on specific properties by providing a filter function.
    /// A filter function is executed for each of the items in the DataSet,
    /// and is called with the item as parameter.
    /// The function must return a boolean.
    /// All items for which the filter function returns true will be emitted.
    /// See section Data Filtering.
    abstract filter: item: 'T -> bool
    /// Order the items by a field name or custom sort function.
    abstract order: U2<string, ('T -> 'T -> float)> option with get, set
    /// Determine the type of output of the get function.
    /// Allowed values are 'Array' | 'Object'.
    /// The default returnType is an Array.
    /// The Object type will return a JSON object with the ID's as keys.
    abstract returnType: DataSelectionOptionsReturnType option with get, set

type [<AllowNullLiteral>] DataView<'T> =
    abstract length: float with get, set

type [<AllowNullLiteral>] DataViewStatic =
    [<Emit "new $0($1...)">] abstract Create: items: ResizeArray<'T> -> DataView<'T>

type DataItemCollectionType =
    U3<ResizeArray<DataItem>, DataSet<DataItem>, DataView<DataItem>>

type DataGroupCollectionType =
    U3<ResizeArray<DataGroup>, DataSet<DataGroup>, DataView<DataGroup>>

type [<AllowNullLiteral>] TitleOption =
    abstract text: string option with get, set
    abstract style: string option with get, set

type [<AllowNullLiteral>] RangeType =
    abstract min: IdType with get, set
    abstract max: IdType with get, set

type [<AllowNullLiteral>] DataAxisSideOption =
    abstract range: RangeType option with get, set
    abstract format: unit -> string
    abstract title: TitleOption option with get, set

type [<AllowNullLiteral>] Graph2dBarChartOption =
    abstract width: float option with get, set
    abstract minWidth: float option with get, set
    abstract sideBySide: bool option with get, set
    abstract align: Graph2dBarChartAlign option with get, set

type [<AllowNullLiteral>] Graph2dDataAxisOption =
    abstract orientation: TimelineOptionsOrientationType option with get, set
    abstract showMinorLabels: bool option with get, set
    abstract showMajorLabels: bool option with get, set
    abstract majorLinesOffset: float option with get, set
    abstract minorLinesOffset: float option with get, set
    abstract labelOffsetX: float option with get, set
    abstract labelOffsetY: float option with get, set
    abstract iconWidth: float option with get, set
    abstract width: string option with get, set
    abstract icons: bool option with get, set
    abstract visible: bool option with get, set
    abstract alignZeros: bool option with get, set
    abstract left: DataAxisSideOption option with get, set
    abstract right: DataAxisSideOption option with get, set

type [<AllowNullLiteral>] Graph2dDrawPointsOption =
    abstract enabled: bool option with get, set
    abstract onRender: unit -> bool
    abstract size: float option with get, set
    abstract style: Graph2dDrawPointsStyle with get, set

type [<AllowNullLiteral>] Graph2dShadedOption =
    abstract orientation: TopBottomEnumType option with get, set
    abstract groupid: IdType option with get, set

type Graph2dOptionBarChart =
    U2<float, Graph2dBarChartOption>

type Graph2dOptionDataAxis =
    U2<bool, Graph2dDataAxisOption>

type Graph2dOptionDrawPoints =
    U2<bool, Graph2dDrawPointsOption>

type Graph2dLegendOption =
    U2<bool, LegendOptions>

type [<AllowNullLiteral>] Graph2dOptions =
    abstract autoResize: bool option with get, set
    abstract barChart: Graph2dOptionBarChart option with get, set
    abstract clickToUse: bool option with get, set
    abstract configure: TimelineOptionsConfigureType option with get, set
    abstract dataAxis: Graph2dOptionDataAxis option with get, set
    abstract defaultGroup: string option with get, set
    abstract drawPoints: Graph2dOptionDrawPoints option with get, set
    abstract ``end``: DateType option with get, set
    abstract format: obj option with get, set
    abstract graphHeight: HeightWidthType option with get, set
    abstract height: HeightWidthType option with get, set
    abstract hiddenDates: obj option with get, set
    abstract legend: Graph2dLegendOption option with get, set
    abstract locale: string option with get, set
    abstract locales: obj option with get, set
    // abstract moment: MomentConstructor option with get, set
    abstract max: DateType option with get, set
    abstract maxHeight: HeightWidthType option with get, set
    abstract maxMinorChars: float option with get, set
    abstract min: DateType option with get, set
    abstract minHeight: HeightWidthType option with get, set
    abstract moveable: bool option with get, set
    abstract multiselect: bool option with get, set
    abstract orientation: string option with get, set
    abstract sampling: bool option with get, set
    abstract showCurrentTime: bool option with get, set
    abstract showMajorLabels: bool option with get, set
    abstract showMinorLabels: bool option with get, set
    abstract sort: bool option with get, set
    abstract stack: bool option with get, set
    abstract start: DateType option with get, set
    abstract style: Graph2dStyleType option with get, set
    abstract throttleRedraw: float option with get, set
    abstract timeAxis: TimelineTimeAxisOption option with get, set
    abstract width: HeightWidthType option with get, set
    abstract yAxisOrientation: RightLeftEnumType option with get, set
    abstract zoomable: bool option with get, set
    abstract zoomKey: string option with get, set
    abstract zoomMax: float option with get, set
    abstract zoomMin: float option with get, set
    abstract zIndex: float option with get, set

type [<AllowNullLiteral>] Graph2d =
    abstract addCustomTime: time: DateType * ?id: IdType -> IdType
    abstract destroy: unit -> unit
    abstract fit: ?options: TimelineAnimationOptions -> unit
    abstract focus: ids: U2<IdType, ResizeArray<IdType>> * ?options: TimelineAnimationOptions -> unit
    abstract getCurrentTime: unit -> DateTime
    abstract getCustomTime: ?id: IdType -> DateTime
    abstract getEventProperties: ``event``: Event -> TimelineEventPropertiesResult
    abstract getItemRange: unit -> obj option
    abstract getSelection: unit -> ResizeArray<IdType>
    abstract getVisibleItems: unit -> ResizeArray<IdType>
    abstract getWindow: unit -> Graph2dGetWindowReturn
    abstract moveTo: time: DateType * ?options: TimelineAnimationOptions -> unit
    abstract on: ``event``: TimelineEvents * callback: (unit -> unit) -> unit
    abstract off: ``event``: TimelineEvents * callback: (unit -> unit) -> unit
    abstract redraw: unit -> unit
    abstract removeCustomTime: id: IdType -> unit
    abstract setCurrentTime: time: DateType -> unit
    abstract setCustomTime: time: DateType * ?id: IdType -> unit
    abstract setCustomTimeTitle: title: string * ?id: IdType -> unit
    abstract setData: data: Graph2dSetDataData -> unit
    abstract setGroups: ?groups: DataGroupCollectionType -> unit
    abstract setItems: items: DataItemCollectionType -> unit
    abstract setOptions: options: TimelineOptions -> unit
    abstract setSelection: ids: U2<IdType, ResizeArray<IdType>> -> unit
    abstract setWindow: start: DateType * ``end``: DateType * ?options: TimelineAnimationOptions -> unit
    abstract setGroups: ?groups: ResizeArray<TimelineGroup> -> unit
    abstract setItems: ?items: ResizeArray<TimelineItem> -> unit
    abstract getLegend: unit -> TimelineWindow
    abstract setWindow: start: obj option * date: obj option -> unit
    abstract focus: selection: obj option -> unit
    abstract on: ?``event``: string * ?callback: (obj option -> unit) -> unit

type [<AllowNullLiteral>] Graph2dGetWindowReturn =
    abstract start: DateTime with get, set
    abstract ``end``: DateTime with get, set

type [<AllowNullLiteral>] Graph2dSetDataData =
    abstract groups: DataGroupCollectionType option with get, set
    abstract items: DataItemCollectionType option with get, set

type [<AllowNullLiteral>] Graph2dStatic =
    [<Emit "new $0($1...)">] abstract Create: container: HTMLElement * items: DataItemCollectionType * groups: DataGroupCollectionType * ?options: Graph2dOptions -> Graph2d
    [<Emit "new $0($1...)">] abstract Create: container: HTMLElement * items: DataItemCollectionType * ?options: Graph2dOptions -> Graph2d

type [<AllowNullLiteral>] Timeline =
    /// Add new vertical bar representing a custom time that can be dragged by the user.
    /// Parameter time can be a Date, Number, or String, and is new Date() by default.
    /// Parameter id can be Number or String and is undefined by default.
    /// The id is added as CSS class name of the custom time bar, allowing to style multiple time bars differently.
    /// The method returns id of the created bar.
    abstract addCustomTime: time: DateType * ?id: IdType -> IdType
    /// Destroy the Timeline. The timeline is removed from memory. all DOM elements and event listeners are cleaned up.
    abstract destroy: unit -> unit
    /// Adjust the visible window such that it fits all items. See also focus(id).
    abstract fit: ?options: TimelineAnimationOptions -> unit
    /// Adjust the visible window such that the selected item (or multiple items) are centered on screen. See also function fit()
    abstract focus: ids: U2<IdType, ResizeArray<IdType>> * ?options: TimelineAnimationOptions -> unit
    /// Get the current time. Only applicable when option showCurrentTime is true.
    abstract getCurrentTime: unit -> DateTime
    /// <summary>Retrieve the custom time from the custom time bar with given id.</summary>
    /// <param name="id">Id is undefined by default.</param>
    abstract getCustomTime: ?id: IdType -> DateTime
    abstract getEventProperties: ``event``: Event -> TimelineEventPropertiesResult
    /// Get the range of all the items as an object containing min date and max date
    abstract getItemRange: unit -> TimelineGetItemRangeReturn
    /// Get an array with the ids of the currently selected items
    abstract getSelection: unit -> ResizeArray<IdType>
    /// Get an array with the ids of the currently visible items.
    abstract getVisibleItems: unit -> ResizeArray<IdType>
    /// Get the current visible window.
    abstract getWindow: unit -> TimelineWindow
    /// Move the window such that given time is centered on screen.
    abstract moveTo: time: DateType * ?options: TimelineAnimationOptions * ?callback: (obj -> unit) -> unit
    /// Create an event listener. The callback function is invoked every time the event is triggered.
    abstract on: ``event``: TimelineEvents * ?callback: (obj -> unit) -> unit
    /// Remove an event listener created before via function on(event[, callback]).
    abstract off: ``event``: TimelineEvents * ?callback: (obj -> unit) -> unit
    /// Force a redraw of the Timeline. The size of all items will be recalculated.
    /// Can be useful to manually redraw when option autoResize=false and the window has been resized, or when the items CSS has been changed.
    abstract redraw: unit -> unit
    /// <summary>Remove vertical bars previously added to the timeline via addCustomTime method.</summary>
    /// <param name="id">ID of the custom vertical bar returned by addCustomTime method.</param>
    abstract removeCustomTime: id: IdType -> unit
    /// Set a current time. This can be used for example to ensure that a client's time is synchronized with a shared server time.
    /// Only applicable when option showCurrentTime is true.
    abstract setCurrentTime: time: DateType -> unit
    /// <summary>Adjust the time of a custom time bar.</summary>
    /// <param name="time">The time the custom time bar should point to</param>
    /// <param name="id">Id of the custom time bar, and is undefined by default.</param>
    abstract setCustomTime: time: DateType * ?id: IdType -> unit
    /// <summary>Adjust the title attribute of a custom time bar.</summary>
    /// <param name="title">The title shown when hover over time bar</param>
    /// <param name="id">Id of the custom time bar, and is undefined by default.</param>
    abstract setCustomTimeTitle: title: string * ?id: IdType -> unit
    /// Set both groups and items at once. Both properties are optional.
    /// This is a convenience method for individually calling both setItems(items) and setGroups(groups).
    /// Both items and groups can be an Array with Objects, a DataSet (offering 2 way data binding), or a DataView (offering 1 way data binding).
    abstract setData: data: TimelineSetDataData -> unit
    /// Set a data set with groups for the Timeline.
    abstract setGroups: ?groups: DataGroupCollectionType -> unit
    /// Set a data set with items for the Timeline.
    abstract setItems: items: DataItemCollectionType -> unit
    /// Set or update options. It is possible to change any option of the timeline at any time.
    /// You can for example switch orientation on the fly.
    abstract setOptions: options: TimelineOptions -> unit
    /// Select one or multiple items by their id. The currently selected items will be unselected.
    /// To unselect all selected items, call `setSelection([])`.
    abstract setSelection: ids: U2<IdType, ResizeArray<IdType>> * ?options: TimelineSetSelectionOptions -> unit
    /// <summary>Set the current visible window.</summary>
    /// <param name="start">If the parameter value of start is null, the parameter will be left unchanged.</param>
    /// <param name="end">If the parameter value of end is null, the parameter will be left unchanged.</param>
    /// <param name="options">Timeline animation options. See {@link TimelineAnimationOptions}</param>
    /// <param name="callback">The callback function</param>
    abstract setWindow: start: DateType * ``end``: DateType * ?options: TimelineAnimationOptions * ?callback: (unit -> unit) -> unit
    /// Toggle rollingMode.
    abstract toggleRollingMode: unit -> unit
    /// <summary>Zoom in the current visible window.</summary>
    /// <param name="percentage">A number and must be between 0 and 1. If null, the window will be left unchanged.</param>
    /// <param name="options">Timeline animation options. See {@link TimelineAnimationOptions}</param>
    /// <param name="callback">The callback function</param>
    abstract zoomIn: percentage: float * ?options: TimelineAnimationOptions * ?callback: (unit -> unit) -> unit
    /// <summary>Zoom out the current visible window.</summary>
    /// <param name="percentage">A number and must be between 0 and 1. If null, the window will be left unchanged.</param>
    /// <param name="options">Timeline animation options. See {@link TimelineAnimationOptions}</param>
    /// <param name="callback">The callback function</param>
    abstract zoomOut: percentage: float * ?options: TimelineAnimationOptions * ?callback: (unit -> unit) -> unit
    abstract setGroups: ?groups: ResizeArray<TimelineGroup> -> unit
    abstract setItems: ?items: ResizeArray<TimelineItem> -> unit
    abstract setWindow: start: obj option * date: obj option -> unit
    abstract focus: selection: obj option -> unit
    abstract on: ?``event``: string * ?callback: (obj option -> unit) -> unit
    abstract off: ``event``: string * ?callback: (obj -> unit) -> unit

type [<AllowNullLiteral>] TimelineGetItemRangeReturn =
    abstract min: DateTime with get, set
    abstract max: DateTime with get, set

type [<AllowNullLiteral>] TimelineSetDataData =
    abstract groups: DataGroupCollectionType option with get, set
    abstract items: DataItemCollectionType option with get, set

type [<AllowNullLiteral>] TimelineSetSelectionOptions =
    abstract focus: bool with get, set
    abstract animation: TimelineAnimationOptions with get, set

type [<AllowNullLiteral>] TimelineStatic =
    [<Emit "new $0($1...)">] abstract Create: container: HTMLElement * items: DataItemCollectionType * groups: DataGroupCollectionType * ?options: TimelineOptions -> Timeline
    [<Emit "new $0($1...)">] abstract Create: container: HTMLElement * items: DataItemCollectionType * ?options: TimelineOptions -> Timeline

type [<AllowNullLiteral>] ITimelineStatic =
    interface end

type [<AllowNullLiteral>] TimelineStaticStatic =
    [<Emit "new $0($1...)">] abstract Create: id: HTMLElement * data: obj option * ?options: obj -> TimelineStatic

type [<AllowNullLiteral>] TimelineWindow =
    abstract start: DateTime with get, set
    abstract ``end``: DateTime with get, set

type [<AllowNullLiteral>] TimelineItemEditableOption =
    abstract remove: bool option with get, set
    abstract updateGroup: bool option with get, set
    abstract updateTime: bool option with get, set

type TimelineItemEditableType =
    U2<bool, TimelineItemEditableOption>

type [<AllowNullLiteral>] TimelineItem =
    abstract className: string option with get, set
    abstract align: TimelineAlignType option with get, set
    abstract content: string with get, set
    abstract ``end``: DateType option with get, set
    abstract group: IdType option with get, set
    abstract id: IdType with get, set
    abstract start: DateType with get, set
    abstract style: string option with get, set
    abstract subgroup: IdType option with get, set
    abstract title: string option with get, set
    abstract ``type``: TimelineItemType option with get, set
    abstract editable: TimelineItemEditableType option with get, set

type [<AllowNullLiteral>] TimelineGroup =
    abstract className: string option with get, set
    abstract content: U2<string, HTMLElement> with get, set
    abstract id: IdType with get, set
    abstract style: string option with get, set
    abstract order: float option with get, set
    abstract subgroupOrder: TimelineOptionsGroupOrderType option with get, set
    abstract title: string option with get, set
    abstract visible: bool option with get, set
    abstract nestedGroups: ResizeArray<IdType> option with get, set
    abstract showNested: bool option with get, set

type [<AllowNullLiteral>] VisSelectProperties =
    abstract items: ResizeArray<float> with get, set

type [<StringEnum>] [<RequireQualifiedAccess>] NetworkEvents =
    | Click
    | DoubleClick
    | Oncontext
    | Hold
    | Release
    | Select
    | SelectNode
    | SelectEdge
    | DeselectNode
    | DeselectEdge
    | DragStart
    | Dragging
    | DragEnd
    | HoverNode
    | BlurNode
    | HoverEdge
    | BlurEdge
    | Zoom
    | ShowPopup
    | HidePopup
    | StartStabilizing
    | StabilizationProgress
    | StabilizationIterationsDone
    | Stabilized
    | Resize
    | InitRedraw
    | BeforeDrawing
    | AfterDrawing
    | AnimationFinished
    | ConfigChange

/// Network is a visualization to display networks and networks consisting of nodes and edges.
/// The visualization is easy to use and supports custom shapes, styles, colors, sizes, images, and more.
/// The network visualization works smooth on any modern browser for up to a few thousand nodes and edges.
/// To handle a larger amount of nodes, Network has clustering support. Network uses HTML canvas for rendering.
type [<AllowNullLiteral>] Network =
    /// Remove the network from the DOM and remove all Hammer bindings and references.
    abstract destroy: unit -> unit
    /// <summary>Override all the data in the network.
    /// If stabilization is enabled in the physics module,
    /// the network will stabilize again.
    /// This method is also performed when first initializing the network.</summary>
    /// <param name="data">network data</param>
    abstract setData: data: Data -> unit
    /// <summary>Set the options.
    /// All available options can be found in the modules above.
    /// Each module requires it's own container with the module name to contain its options.</summary>
    /// <param name="options">network options</param>
    abstract setOptions: options: Options -> unit
    /// <summary>Set an event listener.
    /// Depending on the type of event you get different parameters for the callback function.</summary>
    /// <param name="eventName">the name of the event, f.e. 'click'</param>
    /// <param name="callback">the callback function that will be raised</param>
    abstract on: eventName: NetworkEvents * callback: (obj -> unit) -> unit
    /// <summary>Remove an event listener.
    /// The function you supply has to be the exact same as the one you used in the on function.
    /// If no function is supplied, all listeners will be removed.</summary>
    /// <param name="eventName">the name of the event, f.e. 'click'</param>
    /// <param name="callback">the exact same callback function that was used when calling 'on'</param>
    abstract off: eventName: NetworkEvents * ?callback: (obj -> unit) -> unit
    /// <summary>Set an event listener only once.
    /// After it has taken place, the event listener will be removed.
    /// Depending on the type of event you get different parameters for the callback function.</summary>
    /// <param name="eventName">the name of the event, f.e. 'click'</param>
    /// <param name="callback">the callback function that will be raised once</param>
    abstract once: eventName: NetworkEvents * callback: (obj -> unit) -> unit
    /// <summary>This function converts canvas coordinates to coordinates on the DOM.
    /// Input and output are in the form of {x:Number, y:Number} (IPosition interface).
    /// The DOM values are relative to the network container.</summary>
    /// <param name="position">the canvas coordinates</param>
    abstract canvasToDOM: position: Position -> Position
    /// <summary>This function converts DOM coordinates to coordinates on the canvas.
    /// Input and output are in the form of {x:Number,y:Number} (IPosition interface).
    /// The DOM values are relative to the network container.</summary>
    /// <param name="position">the DOM coordinates</param>
    abstract DOMtoCanvas: position: Position -> Position
    /// Redraw the network.
    abstract redraw: unit -> unit
    /// <summary>Set the size of the canvas.
    /// This is automatically done on a window resize.</summary>
    /// <param name="width">width in a common format, f.e. '100px'</param>
    /// <param name="height">height in a common format, f.e. '100px'</param>
    abstract setSize: width: string * height: string -> unit
    /// The joinCondition function is presented with all nodes.
    abstract cluster: ?options: ClusterOptions -> unit
    /// <summary>This method looks at the provided node and makes a cluster of it and all it's connected nodes.
    /// The behaviour can be customized by proving the options object.
    /// All options of this object are explained below.
    /// The joinCondition is only presented with the connected nodes.</summary>
    /// <param name="nodeId">the id of the node</param>
    /// <param name="options">the cluster options</param>
    abstract clusterByConnection: nodeId: string * ?options: ClusterOptions -> unit
    /// <summary>This method checks all nodes in the network and those with a equal or higher
    /// amount of edges than specified with the hubsize qualify.
    /// If a hubsize is not defined, the hubsize will be determined as the average
    /// value plus two standard deviations.
    /// For all qualifying nodes, clusterByConnection is performed on each of them.
    /// The options object is described for clusterByConnection and does the same here.</summary>
    /// <param name="hubsize">optional hubsize</param>
    /// <param name="options">optional cluster options</param>
    abstract clusterByHubsize: ?hubsize: float * ?options: ClusterOptions -> unit
    /// <summary>This method will cluster all nodes with 1 edge with their respective connected node.</summary>
    /// <param name="options">optional cluster options</param>
    abstract clusterOutliers: ?options: ClusterOptions -> unit
    /// <summary>Nodes can be in clusters.
    /// Clusters can also be in clusters.
    /// This function returns an array of nodeIds showing where the node is.
    ///
    /// Example:
    /// cluster 'A' contains cluster 'B', cluster 'B' contains cluster 'C',
    /// cluster 'C' contains node 'fred'.
    ///
    /// network.clustering.findNode('fred') will return ['A','B','C','fred'].</summary>
    /// <param name="nodeId">the node id.</param>
    abstract findNode: nodeId: IdType -> ResizeArray<IdType>
    /// <summary>Similar to findNode in that it returns all the edge ids that were
    /// created from the provided edge during clustering.</summary>
    /// <param name="baseEdgeId">the base edge id</param>
    abstract getClusteredEdges: baseEdgeId: IdType -> ResizeArray<IdType>
    /// When a clusteredEdgeId is available, this method will return the original
    /// baseEdgeId provided in data.edges ie.
    /// After clustering the 'SelectEdge' event is fired but provides only the clustered edge.
    /// This method can then be used to return the baseEdgeId.
    abstract getBaseEdge: clusteredEdgeId: IdType -> IdType
    /// For the given clusteredEdgeId, this method will return all the original
    /// base edge id's provided in data.edges.
    /// For a non-clustered (i.e. 'base') edge, clusteredEdgeId is returned.
    /// Only the base edge id's are returned.
    /// All clustered edges id's under clusteredEdgeId are skipped,
    /// but scanned recursively to return their base id's.
    abstract getBaseEdges: clusteredEdgeId: IdType -> ResizeArray<IdType>
    /// Visible edges between clustered nodes are not the same edge as the ones provided
    /// in data.edges passed on network creation. With each layer of clustering, copies of
    /// the edges between clusters are created and the previous edges are hidden,
    /// until the cluster is opened. This method takes an edgeId (ie. a base edgeId from data.edges)
    /// and applys the options to it and any edges that were created from it while clustering.
    abstract updateEdge: startEdgeId: IdType * ?options: EdgeOptions -> unit
    /// Clustered Nodes when created are not contained in the original data.nodes
    /// passed on network creation. This method updates the cluster node.
    abstract updateClusteredNode: clusteredNodeId: IdType * ?options: NodeOptions -> unit
    /// <summary>Returns true if the node whose ID has been supplied is a cluster.</summary>
    /// <param name="nodeId">the node id.</param>
    abstract isCluster: nodeId: IdType -> bool
    /// <summary>Returns an array of all nodeIds of the nodes that
    /// would be released if you open the cluster.</summary>
    /// <param name="clusterNodeId">the id of the cluster node</param>
    abstract getNodesInCluster: clusterNodeId: IdType -> ResizeArray<IdType>
    /// <summary>Opens the cluster, releases the contained nodes and edges,
    /// removing the cluster node and cluster edges.
    /// The options object is optional and currently supports one option,
    /// releaseFunction, which is a function that can be used to manually
    /// position the nodes after the cluster is opened.</summary>
    /// <param name="nodeId">the node id</param>
    /// <param name="options">optional open cluster options</param>
    abstract openCluster: nodeId: IdType * ?options: OpenClusterOptions -> unit
    /// If you like the layout of your network
    /// and would like it to start in the same way next time,
    /// ask for the seed using this method and put it in the layout.randomSeed option.
    abstract getSeed: unit -> float
    /// Programatically enable the edit mode.
    /// Similar effect to pressing the edit button.
    abstract enableEditMode: unit -> unit
    /// Programatically disable the edit mode.
    /// Similar effect to pressing the close icon (small cross in the corner of the toolbar).
    abstract disableEditMode: unit -> unit
    /// Go into addNode mode. Having edit mode or manipulation enabled is not required.
    /// To get out of this mode, call disableEditMode().
    /// The callback functions defined in handlerFunctions still apply.
    /// To use these methods without having the manipulation GUI, make sure you set enabled to false.
    abstract addNodeMode: unit -> unit
    /// Edit the selected node.
    /// The explaination from addNodeMode applies here as well.
    abstract editNode: unit -> unit
    /// Go into addEdge mode.
    /// The explaination from addNodeMode applies here as well.
    abstract addEdgeMode: unit -> unit
    /// Go into editEdge mode.
    /// The explaination from addNodeMode applies here as well.
    abstract editEdgeMode: unit -> unit
    /// Delete selected.
    /// Having edit mode or manipulation enabled is not required.
    abstract deleteSelected: unit -> unit
    /// Returns the x y positions in canvas space of the nodes with the supplied nodeIds as an object.
    ///
    /// Alternative inputs are a String containing a nodeId or nothing.
    /// When a String is supplied, the position of the node corresponding to the ID is returned.
    /// When nothing is supplied, the positions of all nodes are returned.
    abstract getPositions: ?nodeIds: ResizeArray<IdType> -> NetworkGetPositionsReturn
    abstract getPositions: nodeId: IdType -> Position
    /// When using the vis.DataSet to load your nodes into the network,
    /// this method will put the X and Y positions of all nodes into that dataset.
    /// If you're loading your nodes from a database and have this dynamically coupled with the DataSet,
    /// you can use this to stablize your network once, then save the positions in that database
    /// through the DataSet so the next time you load the nodes, stabilization will be near instantaneous.
    ///
    /// If the nodes are still moving and you're using dynamic smooth edges (which is on by default),
    /// you can use the option stabilization.onlyDynamicEdges in the physics module to improve initialization time.
    ///
    /// This method does not support clustering.
    /// At the moment it is not possible to cache positions when using clusters since
    /// they cannot be correctly initialized from just the positions.
    abstract storePositions: unit -> unit
    /// <summary>You can use this to programatically move a node.
    /// The supplied x and y positions have to be in canvas space!</summary>
    /// <param name="nodeId">the node that will be moved</param>
    /// <param name="x">new canvas space x position</param>
    /// <param name="y">new canvas space y position</param>
    abstract moveNode: nodeId: IdType * x: float * y: float -> unit
    /// Returns a bounding box for the node including label.
    abstract getBoundingBox: nodeId: IdType -> BoundingBox
    /// <summary>Returns an array of nodeIds of the all the nodes that are directly connected to this node.
    /// If you supply an edgeId, vis will first match the id to nodes.
    /// If no match is found, it will search in the edgelist and return an array: [fromId, toId].</summary>
    /// <param name="nodeOrEdgeId">a node or edge id</param>
    abstract getConnectedNodes: nodeOrEdgeId: IdType * ?direction: DirectionType -> U2<ResizeArray<IdType>, Array<NetworkGetConnectedNodesArray>>
    /// <summary>Returns an array of edgeIds of the edges connected to this node.</summary>
    /// <param name="nodeId">the node id</param>
    abstract getConnectedEdges: nodeId: IdType -> ResizeArray<IdType>
    /// Start the physics simulation.
    /// This is normally done whenever needed and is only really useful
    /// if you stop the simulation yourself and wish to continue it afterwards.
    abstract startSimulation: unit -> unit
    /// This stops the physics simulation and triggers a stabilized event.
    /// Tt can be restarted by dragging a node,
    /// altering the dataset or calling startSimulation().
    abstract stopSimulation: unit -> unit
    /// <summary>You can manually call stabilize at any time.
    /// All the stabilization options above are used.
    /// You can optionally supply the number of iterations it should do.</summary>
    /// <param name="iterations">the number of iterations it should do</param>
    abstract stabilize: ?iterations: float -> unit
    /// Returns an object with selected nodes and edges ids.
    abstract getSelection: unit -> NetworkGetSelectionReturn
    /// Returns an array of selected node ids like so:
    /// [nodeId1, nodeId2, ..].
    abstract getSelectedNodes: unit -> ResizeArray<IdType>
    /// Returns an array of selected edge ids like so:
    /// [edgeId1, edgeId2, ..].
    abstract getSelectedEdges: unit -> ResizeArray<IdType>
    /// Returns a nodeId or undefined.
    /// The DOM positions are expected to be in pixels from the top left corner of the canvas.
    abstract getNodeAt: position: Position -> IdType
    /// Returns a edgeId or undefined.
    /// The DOM positions are expected to be in pixels from the top left corner of the canvas.
    abstract getEdgeAt: position: Position -> IdType
    /// Selects the nodes corresponding to the id's in the input array.
    /// If highlightEdges is true or undefined, the neighbouring edges will also be selected.
    /// This method unselects all other objects before selecting its own objects. Does not fire events.
    abstract selectNodes: nodeIds: ResizeArray<IdType> * ?highlightEdges: bool -> unit
    /// Selects the edges corresponding to the id's in the input array.
    /// This method unselects all other objects before selecting its own objects.
    /// Does not fire events.
    abstract selectEdges: edgeIds: ResizeArray<IdType> -> unit
    /// Sets the selection.
    /// You can also pass only nodes or edges in selection object.
    abstract setSelection: selection: NetworkSetSelectionSelection * ?options: SelectionOptions -> unit
    /// Unselect all objects.
    /// Does not fire events.
    abstract unselectAll: unit -> unit
    /// Returns the current scale of the network.
    /// 1.0 is comparible to 100%, 0 is zoomed out infinitely.
    abstract getScale: unit -> float
    /// Returns the current central focus point of the view in the form: { x: {Number}, y: {Number} }
    abstract getViewPosition: unit -> Position
    /// <summary>Zooms out so all nodes fit on the canvas.</summary>
    /// <param name="options">All options are optional for the fit method</param>
    abstract fit: ?options: FitOptions -> unit
    /// You can focus on a node with this function.
    /// What that means is the view will lock onto that node, if it is moving, the view will also move accordingly.
    /// If the view is dragged by the user, the focus is broken. You can supply options to customize the effect.
    abstract focus: nodeId: IdType * ?options: FocusOptions -> unit
    /// You can animate or move the camera using the moveTo method.
    abstract moveTo: options: MoveToOptions -> unit
    /// Programatically release the focussed node.
    abstract releaseNode: unit -> unit
    /// If you use the configurator, you can call this method to get an options object that contains
    /// all differences from the default options caused by users interacting with the configurator.
    abstract getOptionsFromConfigurator: unit -> obj option

type [<AllowNullLiteral>] NetworkGetPositionsReturn =
    [<Emit "$0[$1]{{=$2}}">] abstract Item: nodeId: string -> Position with get, set

type [<AllowNullLiteral>] NetworkGetSelectionReturn =
    abstract nodes: ResizeArray<IdType> with get, set
    abstract edges: ResizeArray<IdType> with get, set

type [<AllowNullLiteral>] NetworkSetSelectionSelection =
    abstract nodes: ResizeArray<IdType> with get, set
    abstract edges: ResizeArray<IdType> with get, set

/// Network is a visualization to display networks and networks consisting of nodes and edges.
/// The visualization is easy to use and supports custom shapes, styles, colors, sizes, images, and more.
/// The network visualization works smooth on any modern browser for up to a few thousand nodes and edges.
/// To handle a larger amount of nodes, Network has clustering support. Network uses HTML canvas for rendering.
type [<AllowNullLiteral>] NetworkStatic =
    /// <summary>Creates an instance of Network.</summary>
    /// <param name="container">the HTML element representing the network container</param>
    /// <param name="data">network data</param>
    /// <param name="options">optional network options</param>
    [<Emit "new $0($1...)">] abstract Create: container: HTMLElement * data: Data * ?options: Options -> Network

/// Options interface for focus function.
type [<AllowNullLiteral>] FocusOptions =
    inherit ViewPortOptions
    /// Locked denotes whether or not the view remains locked to
    /// the node once the zoom-in animation is finished.
    /// Default value is true.
    abstract locked: bool option with get, set

/// Base options interface for some viewport functions.
type [<AllowNullLiteral>] ViewPortOptions =
    /// The scale is the target zoomlevel.
    /// Default value is 1.0.
    abstract scale: float option with get, set
    /// The offset (in DOM units) is how many pixels from the center the view is focussed.
    /// Default value is {x:0,y:0}
    abstract offset: Position option with get, set
    /// For animation you can either use a Boolean to use it with the default options or
    /// disable it or you can define the duration (in milliseconds) and easing function manually.
    abstract animation: U2<AnimationOptions, bool> option with get, set

/// You will have to define at least a scale, position or offset.
/// Otherwise, there is nothing to move to.
type [<AllowNullLiteral>] MoveToOptions =
    inherit ViewPortOptions
    /// The position (in canvas units!) is the position of the central focus point of the camera.
    abstract position: Position option with get, set

/// Animation options interface.
type [<AllowNullLiteral>] AnimationOptions =
    /// The duration (in milliseconds).
    abstract duration: float with get, set
    /// The easing function.
    ///
    /// Available are:
    /// linear, easeInQuad, easeOutQuad, easeInOutQuad, easeInCubic,
    /// easeOutCubic, easeInOutCubic, easeInQuart, easeOutQuart, easeInOutQuart,
    /// easeInQuint, easeOutQuint, easeInOutQuint.
    abstract easingFunction: EasingFunction with get, set

type [<StringEnum>] [<RequireQualifiedAccess>] EasingFunction =
    | Linear
    | EaseInQuad
    | EaseOutQuad
    | EaseInOutQuad
    | EaseInCubic
    | EaseOutCubic
    | EaseInOutCubic
    | EaseInQuart
    | EaseOutQuart
    | EaseInOutQuart
    | EaseInQuint
    | EaseOutQuint
    | EaseInOutQuint

/// Optional options for the fit method.
type [<AllowNullLiteral>] FitOptions =
    /// The nodes can be used to zoom to fit only specific nodes in the view.
    abstract nodes: ResizeArray<string> option with get, set
    /// For animation you can either use a Boolean to use it with the default options or
    /// disable it or you can define the duration (in milliseconds) and easing function manually.
    abstract animation: TimelineAnimationType with get, set

type [<AllowNullLiteral>] SelectionOptions =
    abstract unselectAll: bool option with get, set
    abstract highlightEdges: bool option with get, set

/// These values are in canvas space.
type [<AllowNullLiteral>] BoundingBox =
    abstract top: float with get, set
    abstract left: float with get, set
    abstract right: float with get, set
    abstract bottom: float with get, set

/// Cluster methods options interface.
type [<AllowNullLiteral>] ClusterOptions =
    /// Optional for all but the cluster method.
    /// The cluster module loops over all nodes that are selected to be in the cluster
    /// and calls this function with their data as argument. If this function returns true,
    /// this node will be added to the cluster. You have access to all options (including the default)
    /// as well as any custom fields you may have added to the node to determine whether or not to include it in the cluster.
    abstract joinCondition: nodeOptions: obj option -> bool
    /// Optional.
    /// Before creating the new cluster node, this (optional) function will be called with the properties
    /// supplied by you (clusterNodeProperties), all contained nodes and all contained edges.
    /// You can use this to update the properties of the cluster based on which items it contains.
    /// The function should return the properties to create the cluster node.
    abstract processProperties: clusterOptions: obj option * childNodesOptions: ResizeArray<obj option> * childEdgesOptions: ResizeArray<obj option> -> obj option
    /// Optional.
    /// This is an object containing the options for the cluster node.
    /// All options described in the nodes module are allowed.
    /// This allows you to style your cluster node any way you want.
    /// This is also the style object that is provided in the processProperties function for fine tuning.
    /// If undefined, default node options will be used.
    abstract clusterNodeProperties: NodeOptions option with get, set
    /// Optional.
    /// This is an object containing the options for the edges connected to the cluster.
    /// All options described in the edges module are allowed.
    /// Using this, you can style the edges connecting to the cluster any way you want.
    /// If none are provided, the options from the edges that are replaced are used.
    /// If undefined, default edge options will be used.
    abstract clusterEdgeProperties: EdgeOptions option with get, set

/// Options for the openCluster function of Network.
type [<AllowNullLiteral>] OpenClusterOptions =
    /// A function that can be used to manually position the nodes after the cluster is opened.
    /// The containedNodesPositions contain the positions of the nodes in the cluster at the
    /// moment they were clustered. This function is expected to return the newPositions,
    /// which can be the containedNodesPositions (altered) or a new object.
    /// This has to be an object with keys equal to the nodeIds that exist in the
    /// containedNodesPositions and an {x:x,y:y} position object.
    ///
    /// For all nodeIds not listed in this returned object,
    /// we will position them at the location of the cluster.
    /// This is also the default behaviour when no releaseFunction is defined.
    abstract releaseFunction: clusterPosition: Position * containedNodesPositions: OpenClusterOptionsReleaseFunctionContainedNodesPositions -> OpenClusterOptionsReleaseFunctionReturn

type [<AllowNullLiteral>] OpenClusterOptionsReleaseFunctionContainedNodesPositions =
    [<Emit "$0[$1]{{=$2}}">] abstract Item: nodeId: string -> Position with get, set

type [<AllowNullLiteral>] OpenClusterOptionsReleaseFunctionReturn =
    [<Emit "$0[$1]{{=$2}}">] abstract Item: nodeId: string -> Position with get, set

type [<AllowNullLiteral>] Position =
    abstract x: float with get, set
    abstract y: float with get, set

type [<AllowNullLiteral>] Properties =
    abstract nodes: ResizeArray<string> with get, set
    abstract edges: ResizeArray<string> with get, set
    abstract ``event``: ResizeArray<string> with get, set
    abstract pointer: PropertiesPointer with get, set
    abstract previousSelection: PropertiesPreviousSelection option with get, set

type [<AllowNullLiteral>] Callback =
    abstract callback: ?``params``: obj -> unit

type [<AllowNullLiteral>] Data =
    abstract nodes: U2<ResizeArray<Node>, DataSet<Node>> option with get, set
    abstract edges: U2<ResizeArray<Edge>, DataSet<Edge>> option with get, set

type [<AllowNullLiteral>] Node =
    inherit NodeOptions
    abstract id: IdType option with get, set

type [<AllowNullLiteral>] Edge =
    inherit EdgeOptions
    abstract from: IdType option with get, set
    abstract ``to``: IdType option with get, set
    abstract id: IdType option with get, set

type [<AllowNullLiteral>] Locales =
    [<Emit "$0[$1]{{=$2}}">] abstract Item: language: string -> LocaleMessages option with get, set
    abstract en: LocaleMessages option with get, set
    abstract cn: LocaleMessages option with get, set
    abstract de: LocaleMessages option with get, set
    abstract es: LocaleMessages option with get, set
    abstract it: LocaleMessages option with get, set
    abstract nl: LocaleMessages option with get, set
    abstract ``pt-br``: LocaleMessages option with get, set
    abstract ru: LocaleMessages option with get, set

type [<AllowNullLiteral>] LocaleMessages =
    abstract edit: string with get, set
    abstract del: string with get, set
    abstract back: string with get, set
    abstract addNode: string with get, set
    abstract addEdge: string with get, set
    abstract editNode: string with get, set
    abstract editEdge: string with get, set
    abstract addDescription: string with get, set
    abstract edgeDescription: string with get, set
    abstract editEdgeDescription: string with get, set
    abstract createEdgeError: string with get, set
    abstract deleteClusterError: string with get, set
    abstract editClusterError: string with get, set

type [<AllowNullLiteral>] Options =
    abstract autoResize: bool option with get, set
    abstract width: string option with get, set
    abstract height: string option with get, set
    abstract locale: string option with get, set
    abstract locales: Locales option with get, set
    abstract clickToUse: bool option with get, set
    abstract configure: obj option with get, set
    abstract edges: EdgeOptions option with get, set
    abstract nodes: NodeOptions option with get, set
    abstract groups: obj option with get, set
    abstract layout: obj option with get, set
    abstract interaction: obj option with get, set
    abstract manipulation: obj option with get, set
    abstract physics: obj option with get, set

type [<AllowNullLiteral>] Image =
    abstract unselected: string option with get, set
    abstract selected: string option with get, set

type [<AllowNullLiteral>] Color =
    abstract border: string option with get, set
    abstract background: string option with get, set
    abstract highlight: U2<string, ColorHighlight> option with get, set
    abstract hover: U2<string, ColorHighlight> option with get, set

type [<AllowNullLiteral>] NodeOptions =
    abstract borderWidth: float option with get, set
    abstract borderWidthSelected: float option with get, set
    abstract brokenImage: string option with get, set
    abstract color: U2<string, Color> option with get, set
    abstract ``fixed``: U2<bool, NodeOptionsFixed> option with get, set
    abstract font: U2<string, NodeOptionsFont> option with get, set
    abstract group: string option with get, set
    abstract hidden: bool option with get, set
    abstract icon: NodeOptionsIcon option with get, set
    abstract image: U2<string, Image> option with get, set
    abstract label: string option with get, set
    abstract labelHighlightBold: bool option with get, set
    abstract level: float option with get, set
    abstract margin: NodeOptionsMargin option with get, set
    abstract mass: float option with get, set
    abstract physics: bool option with get, set
    abstract scaling: OptionsScaling option with get, set
    abstract shadow: U2<bool, OptionsShadow> option with get, set
    abstract shape: string option with get, set
    abstract shapeProperties: NodeOptionsShapeProperties option with get, set
    abstract size: float option with get, set
    abstract title: string option with get, set
    abstract value: float option with get, set
    /// If false, no widthConstraint is applied. If a number is specified, the minimum and maximum widths of the node are set to the value.
    /// The node's label's lines will be broken on spaces to stay below the maximum and the node's width
    /// will be set to the minimum if less than the value.
    abstract widthConstraint: U3<float, bool, NodeOptionsWidthConstraint> option with get, set
    abstract x: float option with get, set
    abstract y: float option with get, set

type [<AllowNullLiteral>] EdgeOptions =
    abstract arrows: U2<string, EdgeOptionsArrows> option with get, set
    abstract arrowStrikethrough: bool option with get, set
    abstract color: U2<string, EdgeOptionsColor> option with get, set
    abstract dashes: U2<bool, ResizeArray<float>> option with get, set
    abstract font: U2<string, NodeOptionsFont> option with get, set
    abstract hidden: bool option with get, set
    abstract hoverWidth: float option with get, set
    abstract label: string option with get, set
    abstract labelHighlightBold: bool option with get, set
    abstract length: float option with get, set
    abstract physics: bool option with get, set
    abstract scaling: OptionsScaling option with get, set
    abstract selectionWidth: float option with get, set
    abstract selfReferenceSize: float option with get, set
    abstract shadow: U2<bool, OptionsShadow> option with get, set
    abstract smooth: U2<bool, EdgeOptionsSmooth> option with get, set
    abstract title: string option with get, set
    abstract value: float option with get, set
    abstract width: float option with get, set

type [<AllowNullLiteral>] FontOptions =
    abstract color: string option with get, set
    abstract size: float option with get, set
    abstract face: string option with get, set
    abstract ``mod``: string option with get, set
    abstract vadjust: string option with get, set

type [<AllowNullLiteral>] OptionsScaling =
    abstract min: float option with get, set
    abstract max: float option with get, set
    abstract label: U2<bool, OptionsScalingLabel> option with get, set
    abstract customScalingFunction: ?min: float * ?max: float * ?total: float * ?value: float -> float

type [<AllowNullLiteral>] OptionsShadow =
    abstract enabled: bool with get, set
    abstract color: string with get, set
    abstract size: float with get, set
    abstract x: float with get, set
    abstract y: float with get, set

type [<StringEnum>] [<RequireQualifiedAccess>] TimelineHiddenDateOptionRepeat =
    | Daily
    | Weekly
    | Monthly
    | Yearly

type [<StringEnum>] [<RequireQualifiedAccess>] TimelineTooltipOptionOverflowMethod =
    | Cap
    | Flip

type [<AllowNullLiteral>] TimelineOptionsTooltipOnItemUpdateTime =
    abstract template: item: obj option -> obj option

type [<StringEnum>] [<RequireQualifiedAccess>] DataSelectionOptionsReturnType =
    | [<CompiledName "Array">] Array
    | [<CompiledName "Object">] Object

type [<AllowNullLiteral>] NetworkGetConnectedNodesArray =
    abstract fromId: IdType with get, set
    abstract toId: IdType with get, set

type [<AllowNullLiteral>] PropertiesPointer =
    abstract DOM: Position with get, set
    abstract canvas: Position with get, set

type [<AllowNullLiteral>] PropertiesPreviousSelection =
    abstract nodes: ResizeArray<string> with get, set
    abstract edges: ResizeArray<string> with get, set

type [<AllowNullLiteral>] ColorHighlight =
    abstract border: string option with get, set
    abstract background: string option with get, set

type [<AllowNullLiteral>] NodeOptionsFixed =
    abstract x: bool option with get, set
    abstract y: bool option with get, set

type [<AllowNullLiteral>] NodeOptionsFont =
    abstract color: string option with get, set
    abstract size: float option with get, set
    abstract face: string option with get, set
    abstract background: string option with get, set
    abstract strokeWidth: float option with get, set
    abstract strokeColor: string option with get, set
    abstract align: string option with get, set
    abstract vadjust: string option with get, set
    abstract multi: string option with get, set
    abstract bold: U2<string, FontOptions> option with get, set
    abstract ital: U2<string, FontOptions> option with get, set
    abstract boldital: U2<string, FontOptions> option with get, set
    abstract mono: U2<string, FontOptions> option with get, set

type [<AllowNullLiteral>] NodeOptionsIcon =
    abstract face: string option with get, set
    abstract code: string option with get, set
    abstract size: float option with get, set
    abstract color: string option with get, set

type [<AllowNullLiteral>] NodeOptionsMargin =
    abstract top: float option with get, set
    abstract right: float option with get, set
    abstract bottom: float option with get, set
    abstract left: float option with get, set

type [<AllowNullLiteral>] NodeOptionsShapeProperties =
    abstract borderDashes: U2<bool, ResizeArray<float>> option with get, set
    abstract borderRadius: float option with get, set
    abstract interpolation: bool option with get, set
    abstract useImageSize: bool option with get, set
    abstract useBorderWithImage: bool option with get, set

type [<AllowNullLiteral>] NodeOptionsWidthConstraint =
    abstract minimum: float option with get, set
    abstract maximum: float option with get, set

type [<AllowNullLiteral>] EdgeOptionsArrowsTo =
    abstract enabled: bool option with get, set
    abstract scaleFactor: float option with get, set
    abstract ``type``: string option with get, set

type [<AllowNullLiteral>] EdgeOptionsArrows =
    abstract ``to``: U2<bool, EdgeOptionsArrowsTo> option with get, set
    abstract middle: U2<bool, EdgeOptionsArrowsTo> option with get, set
    abstract from: U2<bool, EdgeOptionsArrowsTo> option with get, set

type [<AllowNullLiteral>] EdgeOptionsColor =
    abstract color: string option with get, set
    abstract highlight: string option with get, set
    abstract hover: string option with get, set
    abstract ``inherit``: U2<bool, string> option with get, set
    abstract opacity: float option with get, set

type [<AllowNullLiteral>] EdgeOptionsSmooth =
    abstract enabled: bool with get, set
    abstract ``type``: string with get, set
    abstract forceDirection: U2<string, bool> option with get, set
    abstract roundness: float with get, set

type [<AllowNullLiteral>] OptionsScalingLabel =
    abstract enabled: bool option with get, set
    abstract min: float option with get, set
    abstract max: float option with get, set
    abstract maxVisible: float option with get, set
    abstract drawThreshold: float option with get, set
