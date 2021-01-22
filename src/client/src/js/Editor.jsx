import React, { useRef, useEffect } from "react";
import MonacoEditor  from "@monaco-editor/react";
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

  useEventListener(window, "select_range", selectRange);

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
  const handleEditorChange = (value) => {
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
      <MonacoEditor
        height={"100%"}
        language={language}
        editorDidMount={handleEditorDidMount}
        value={value}
        onChange={handleEditorChange}
        options={options}
      />
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
