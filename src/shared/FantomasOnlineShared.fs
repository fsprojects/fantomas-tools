module FantomasOnline.Shared

type FantomasOption =
    | IntOption of order:int * name:string * value:int
    | BoolOption of order:int * name:string * value:bool

let sortByOption = function | IntOption (o,_,_) | BoolOption (o,_,_) -> o

type FormatRequest =
    { SourceCode: string
      Options: FantomasOption list
      IsFsi: bool }
