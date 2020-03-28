import React, { useRef, useEffect } from "react";
import { ControlledEditor as MonacoEditor } from "@monaco-editor/react";
import PropTypes from "prop-types";

const useEventListener = (target, type, listener, ...options) => {
  useEffect(() => {
    target.addEventListener(type, listener, ...options);
    return () => {
      target.removeEventListener(type, listener, ...options);
    };
  }, [target, type, listener, options]);
};

const Editor = ({ onChange, language, getEditor, value, isReadOnly }) => {
  const editorRef = useRef(null);

  function selectRange(ev) {
    if (ev.detail && editorRef.current) {
      const range = ev.detail;
      const editor = editorRef.current;
      editor.setSelection(range);
      editor.revealRangeInCenter(range, 0);
    }
  }

  useEventListener(window, "trivia_select_range", selectRange);

  const options = {
    readOnly: isReadOnly,
    selectOnLineNumbers: true,
    lineNumbers: true,
    theme: "vs-light",
    renderWhitespace: "all",
    minimap: {
      enabled: false
    }
  };
  const handleEditorChange = (ev, value) => {
    if (onChange) {
      onChange(value);
    }
    return value;
  };

  function handleEditorDidMount(kel, editor) {
    editorRef.current = editor;
    if (getEditor) {
      getEditor(editor);
    }
  }

  return (
    <div style={{ height: "100%", overflow: "hidden" }}>
      <MonacoEditor
        height={"100%"}
        language={language}
        editorDidMount={handleEditorDidMount}
        value={value}
        onChange={handleEditorChange}
        options={options}
      />
    </div>
  );
};

Editor.propTypes = {
  onChange: PropTypes.func,
  value: PropTypes.string,
  language: PropTypes.string,
  isReadOnly: PropTypes.bool,
  getEditor: PropTypes.func
};

Editor.defaultProps = {
  value: "",
  language: "fsharp",
  isReadOnly: false
};

export default Editor;
