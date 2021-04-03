import { r as react } from './common/index-59cd3494.js';
import { p as propTypes$g } from './common/index-e2fb70cf.js';
import { c as createCommonjsModule } from './common/_commonjsHelpers-668e6127.js';

function _extends() {
  _extends = Object.assign || function (target) {
    for (var i = 1; i < arguments.length; i++) {
      var source = arguments[i];

      for (var key in source) {
        if (Object.prototype.hasOwnProperty.call(source, key)) {
          target[key] = source[key];
        }
      }
    }

    return target;
  };

  return _extends.apply(this, arguments);
}

function _objectWithoutPropertiesLoose(source, excluded) {
  if (source == null) return {};
  var target = {};
  var sourceKeys = Object.keys(source);
  var key, i;

  for (i = 0; i < sourceKeys.length; i++) {
    key = sourceKeys[i];
    if (excluded.indexOf(key) >= 0) continue;
    target[key] = source[key];
  }

  return target;
}

var classnames = createCommonjsModule(function (module) {
/*!
  Copyright (c) 2017 Jed Watson.
  Licensed under the MIT License (MIT), see
  http://jedwatson.github.io/classnames
*/
/* global define */

(function () {

	var hasOwn = {}.hasOwnProperty;

	function classNames () {
		var classes = [];

		for (var i = 0; i < arguments.length; i++) {
			var arg = arguments[i];
			if (!arg) continue;

			var argType = typeof arg;

			if (argType === 'string' || argType === 'number') {
				classes.push(arg);
			} else if (Array.isArray(arg) && arg.length) {
				var inner = classNames.apply(null, arg);
				if (inner) {
					classes.push(inner);
				}
			} else if (argType === 'object') {
				for (var key in arg) {
					if (hasOwn.call(arg, key) && arg[key]) {
						classes.push(key);
					}
				}
			}
		}

		return classes.join(' ');
	}

	if ( module.exports) {
		classNames.default = classNames;
		module.exports = classNames;
	} else {
		window.classNames = classNames;
	}
}());
});

var globalCssModule;
function mapToCssModules(className, cssModule) {
  if (className === void 0) {
    className = '';
  }

  if (cssModule === void 0) {
    cssModule = globalCssModule;
  }

  if (!cssModule) return className;
  return className.split(' ').map(function (c) {
    return cssModule[c] || c;
  }).join(' ');
}
/**
 * Returns a new object with the key/value pairs from `obj` that are not in the array `omitKeys`.
 */

function omit(obj, omitKeys) {
  var result = {};
  Object.keys(obj).forEach(function (key) {
    if (omitKeys.indexOf(key) === -1) {
      result[key] = obj[key];
    }
  });
  return result;
}
var warned = {};
function warnOnce(message) {
  if (!warned[message]) {
    /* istanbul ignore else */
    if (typeof console !== 'undefined') {
      console.error(message); // eslint-disable-line no-console
    }

    warned[message] = true;
  }
}

var Element = typeof window === 'object' && window.Element || function () {};

function DOMElement(props, propName, componentName) {
  if (!(props[propName] instanceof Element)) {
    return new Error('Invalid prop `' + propName + '` supplied to `' + componentName + '`. Expected prop to be an instance of Element. Validation failed.');
  }
}
var targetPropType = propTypes$g.oneOfType([propTypes$g.string, propTypes$g.func, DOMElement, propTypes$g.shape({
  current: propTypes$g.any
})]);
var tagPropType = propTypes$g.oneOfType([propTypes$g.func, propTypes$g.string, propTypes$g.shape({
  $$typeof: propTypes$g.symbol,
  render: propTypes$g.func
}), propTypes$g.arrayOf(propTypes$g.oneOfType([propTypes$g.func, propTypes$g.string, propTypes$g.shape({
  $$typeof: propTypes$g.symbol,
  render: propTypes$g.func
})]))]);
function isObject(value) {
  var type = typeof value;
  return value != null && (type === 'object' || type === 'function');
}

var rowColWidths = ['xs', 'sm', 'md', 'lg', 'xl'];
var rowColsPropType = propTypes$g.oneOfType([propTypes$g.number, propTypes$g.string]);
var propTypes = {
  tag: tagPropType,
  noGutters: propTypes$g.bool,
  className: propTypes$g.string,
  cssModule: propTypes$g.object,
  form: propTypes$g.bool,
  xs: rowColsPropType,
  sm: rowColsPropType,
  md: rowColsPropType,
  lg: rowColsPropType,
  xl: rowColsPropType
};
var defaultProps = {
  tag: 'div',
  widths: rowColWidths
};

var Row = function Row(props) {
  var className = props.className,
      cssModule = props.cssModule,
      noGutters = props.noGutters,
      Tag = props.tag,
      form = props.form,
      widths = props.widths,
      attributes = _objectWithoutPropertiesLoose(props, ["className", "cssModule", "noGutters", "tag", "form", "widths"]);

  var colClasses = [];
  widths.forEach(function (colWidth, i) {
    var colSize = props[colWidth];
    delete attributes[colWidth];

    if (!colSize) {
      return;
    }

    var isXs = !i;
    colClasses.push(isXs ? "row-cols-" + colSize : "row-cols-" + colWidth + "-" + colSize);
  });
  var classes = mapToCssModules(classnames(className, noGutters ? 'no-gutters' : null, form ? 'form-row' : 'row', colClasses), cssModule);
  return /*#__PURE__*/react.createElement(Tag, _extends({}, attributes, {
    className: classes
  }));
};

Row.propTypes = propTypes;
Row.defaultProps = defaultProps;

var colWidths = ['xs', 'sm', 'md', 'lg', 'xl'];
var stringOrNumberProp = propTypes$g.oneOfType([propTypes$g.number, propTypes$g.string]);
var columnProps = propTypes$g.oneOfType([propTypes$g.bool, propTypes$g.number, propTypes$g.string, propTypes$g.shape({
  size: propTypes$g.oneOfType([propTypes$g.bool, propTypes$g.number, propTypes$g.string]),
  order: stringOrNumberProp,
  offset: stringOrNumberProp
})]);
var propTypes$1 = {
  tag: tagPropType,
  xs: columnProps,
  sm: columnProps,
  md: columnProps,
  lg: columnProps,
  xl: columnProps,
  className: propTypes$g.string,
  cssModule: propTypes$g.object,
  widths: propTypes$g.array
};
var defaultProps$1 = {
  tag: 'div',
  widths: colWidths
};

var getColumnSizeClass = function getColumnSizeClass(isXs, colWidth, colSize) {
  if (colSize === true || colSize === '') {
    return isXs ? 'col' : "col-" + colWidth;
  } else if (colSize === 'auto') {
    return isXs ? 'col-auto' : "col-" + colWidth + "-auto";
  }

  return isXs ? "col-" + colSize : "col-" + colWidth + "-" + colSize;
};

var Col = function Col(props) {
  var className = props.className,
      cssModule = props.cssModule,
      widths = props.widths,
      Tag = props.tag,
      attributes = _objectWithoutPropertiesLoose(props, ["className", "cssModule", "widths", "tag"]);

  var colClasses = [];
  widths.forEach(function (colWidth, i) {
    var columnProp = props[colWidth];
    delete attributes[colWidth];

    if (!columnProp && columnProp !== '') {
      return;
    }

    var isXs = !i;

    if (isObject(columnProp)) {
      var _classNames;

      var colSizeInterfix = isXs ? '-' : "-" + colWidth + "-";
      var colClass = getColumnSizeClass(isXs, colWidth, columnProp.size);
      colClasses.push(mapToCssModules(classnames((_classNames = {}, _classNames[colClass] = columnProp.size || columnProp.size === '', _classNames["order" + colSizeInterfix + columnProp.order] = columnProp.order || columnProp.order === 0, _classNames["offset" + colSizeInterfix + columnProp.offset] = columnProp.offset || columnProp.offset === 0, _classNames)), cssModule));
    } else {
      var _colClass = getColumnSizeClass(isXs, colWidth, columnProp);

      colClasses.push(_colClass);
    }
  });

  if (!colClasses.length) {
    colClasses.push('col');
  }

  var classes = mapToCssModules(classnames(className, colClasses), cssModule);
  return /*#__PURE__*/react.createElement(Tag, _extends({}, attributes, {
    className: classes
  }));
};

Col.propTypes = propTypes$1;
Col.defaultProps = defaultProps$1;

var propTypes$2 = {
  light: propTypes$g.bool,
  dark: propTypes$g.bool,
  full: propTypes$g.bool,
  fixed: propTypes$g.string,
  sticky: propTypes$g.string,
  color: propTypes$g.string,
  role: propTypes$g.string,
  tag: tagPropType,
  className: propTypes$g.string,
  cssModule: propTypes$g.object,
  expand: propTypes$g.oneOfType([propTypes$g.bool, propTypes$g.string])
};
var defaultProps$2 = {
  tag: 'nav',
  expand: false
};

var getExpandClass = function getExpandClass(expand) {
  if (expand === false) {
    return false;
  } else if (expand === true || expand === 'xs') {
    return 'navbar-expand';
  }

  return "navbar-expand-" + expand;
};

var Navbar = function Navbar(props) {
  var _classNames;

  var expand = props.expand,
      className = props.className,
      cssModule = props.cssModule,
      light = props.light,
      dark = props.dark,
      fixed = props.fixed,
      sticky = props.sticky,
      color = props.color,
      Tag = props.tag,
      attributes = _objectWithoutPropertiesLoose(props, ["expand", "className", "cssModule", "light", "dark", "fixed", "sticky", "color", "tag"]);

  var classes = mapToCssModules(classnames(className, 'navbar', getExpandClass(expand), (_classNames = {
    'navbar-light': light,
    'navbar-dark': dark
  }, _classNames["bg-" + color] = color, _classNames["fixed-" + fixed] = fixed, _classNames["sticky-" + sticky] = sticky, _classNames)), cssModule);
  return /*#__PURE__*/react.createElement(Tag, _extends({}, attributes, {
    className: classes
  }));
};

Navbar.propTypes = propTypes$2;
Navbar.defaultProps = defaultProps$2;

var propTypes$3 = {
  tag: tagPropType,
  className: propTypes$g.string,
  cssModule: propTypes$g.object
};
var defaultProps$3 = {
  tag: 'a'
};

var NavbarBrand = function NavbarBrand(props) {
  var className = props.className,
      cssModule = props.cssModule,
      Tag = props.tag,
      attributes = _objectWithoutPropertiesLoose(props, ["className", "cssModule", "tag"]);

  var classes = mapToCssModules(classnames(className, 'navbar-brand'), cssModule);
  return /*#__PURE__*/react.createElement(Tag, _extends({}, attributes, {
    className: classes
  }));
};

NavbarBrand.propTypes = propTypes$3;
NavbarBrand.defaultProps = defaultProps$3;

var propTypes$4 = {
  tabs: propTypes$g.bool,
  pills: propTypes$g.bool,
  vertical: propTypes$g.oneOfType([propTypes$g.bool, propTypes$g.string]),
  horizontal: propTypes$g.string,
  justified: propTypes$g.bool,
  fill: propTypes$g.bool,
  navbar: propTypes$g.bool,
  card: propTypes$g.bool,
  tag: tagPropType,
  className: propTypes$g.string,
  cssModule: propTypes$g.object
};
var defaultProps$4 = {
  tag: 'ul',
  vertical: false
};

var getVerticalClass = function getVerticalClass(vertical) {
  if (vertical === false) {
    return false;
  } else if (vertical === true || vertical === 'xs') {
    return 'flex-column';
  }

  return "flex-" + vertical + "-column";
};

var Nav = function Nav(props) {
  var className = props.className,
      cssModule = props.cssModule,
      tabs = props.tabs,
      pills = props.pills,
      vertical = props.vertical,
      horizontal = props.horizontal,
      justified = props.justified,
      fill = props.fill,
      navbar = props.navbar,
      card = props.card,
      Tag = props.tag,
      attributes = _objectWithoutPropertiesLoose(props, ["className", "cssModule", "tabs", "pills", "vertical", "horizontal", "justified", "fill", "navbar", "card", "tag"]);

  var classes = mapToCssModules(classnames(className, navbar ? 'navbar-nav' : 'nav', horizontal ? "justify-content-" + horizontal : false, getVerticalClass(vertical), {
    'nav-tabs': tabs,
    'card-header-tabs': card && tabs,
    'nav-pills': pills,
    'card-header-pills': card && pills,
    'nav-justified': justified,
    'nav-fill': fill
  }), cssModule);
  return /*#__PURE__*/react.createElement(Tag, _extends({}, attributes, {
    className: classes
  }));
};

Nav.propTypes = propTypes$4;
Nav.defaultProps = defaultProps$4;

var propTypes$5 = {
  tag: tagPropType,
  active: propTypes$g.bool,
  className: propTypes$g.string,
  cssModule: propTypes$g.object
};
var defaultProps$5 = {
  tag: 'li'
};

var NavItem = function NavItem(props) {
  var className = props.className,
      cssModule = props.cssModule,
      active = props.active,
      Tag = props.tag,
      attributes = _objectWithoutPropertiesLoose(props, ["className", "cssModule", "active", "tag"]);

  var classes = mapToCssModules(classnames(className, 'nav-item', active ? 'active' : false), cssModule);
  return /*#__PURE__*/react.createElement(Tag, _extends({}, attributes, {
    className: classes
  }));
};

NavItem.propTypes = propTypes$5;
NavItem.defaultProps = defaultProps$5;

function _assertThisInitialized(self) {
  if (self === void 0) {
    throw new ReferenceError("this hasn't been initialised - super() hasn't been called");
  }

  return self;
}

function _inheritsLoose(subClass, superClass) {
  subClass.prototype = Object.create(superClass.prototype);
  subClass.prototype.constructor = subClass;
  subClass.__proto__ = superClass;
}

var propTypes$6 = {
  tag: tagPropType,
  innerRef: propTypes$g.oneOfType([propTypes$g.object, propTypes$g.func, propTypes$g.string]),
  disabled: propTypes$g.bool,
  active: propTypes$g.bool,
  className: propTypes$g.string,
  cssModule: propTypes$g.object,
  onClick: propTypes$g.func,
  href: propTypes$g.any
};
var defaultProps$6 = {
  tag: 'a'
};

var NavLink = /*#__PURE__*/function (_React$Component) {
  _inheritsLoose(NavLink, _React$Component);

  function NavLink(props) {
    var _this;

    _this = _React$Component.call(this, props) || this;
    _this.onClick = _this.onClick.bind(_assertThisInitialized(_this));
    return _this;
  }

  var _proto = NavLink.prototype;

  _proto.onClick = function onClick(e) {
    if (this.props.disabled) {
      e.preventDefault();
      return;
    }

    if (this.props.href === '#') {
      e.preventDefault();
    }

    if (this.props.onClick) {
      this.props.onClick(e);
    }
  };

  _proto.render = function render() {
    var _this$props = this.props,
        className = _this$props.className,
        cssModule = _this$props.cssModule,
        active = _this$props.active,
        Tag = _this$props.tag,
        innerRef = _this$props.innerRef,
        attributes = _objectWithoutPropertiesLoose(_this$props, ["className", "cssModule", "active", "tag", "innerRef"]);

    var classes = mapToCssModules(classnames(className, 'nav-link', {
      disabled: attributes.disabled,
      active: active
    }), cssModule);
    return /*#__PURE__*/react.createElement(Tag, _extends({}, attributes, {
      ref: innerRef,
      onClick: this.onClick,
      className: classes
    }));
  };

  return NavLink;
}(react.Component);

NavLink.propTypes = propTypes$6;
NavLink.defaultProps = defaultProps$6;

var propTypes$7 = {
  active: propTypes$g.bool,
  'aria-label': propTypes$g.string,
  block: propTypes$g.bool,
  color: propTypes$g.string,
  disabled: propTypes$g.bool,
  outline: propTypes$g.bool,
  tag: tagPropType,
  innerRef: propTypes$g.oneOfType([propTypes$g.object, propTypes$g.func, propTypes$g.string]),
  onClick: propTypes$g.func,
  size: propTypes$g.string,
  children: propTypes$g.node,
  className: propTypes$g.string,
  cssModule: propTypes$g.object,
  close: propTypes$g.bool
};
var defaultProps$7 = {
  color: 'secondary',
  tag: 'button'
};

var Button = /*#__PURE__*/function (_React$Component) {
  _inheritsLoose(Button, _React$Component);

  function Button(props) {
    var _this;

    _this = _React$Component.call(this, props) || this;
    _this.onClick = _this.onClick.bind(_assertThisInitialized(_this));
    return _this;
  }

  var _proto = Button.prototype;

  _proto.onClick = function onClick(e) {
    if (this.props.disabled) {
      e.preventDefault();
      return;
    }

    if (this.props.onClick) {
      return this.props.onClick(e);
    }
  };

  _proto.render = function render() {
    var _this$props = this.props,
        active = _this$props.active,
        ariaLabel = _this$props['aria-label'],
        block = _this$props.block,
        className = _this$props.className,
        close = _this$props.close,
        cssModule = _this$props.cssModule,
        color = _this$props.color,
        outline = _this$props.outline,
        size = _this$props.size,
        Tag = _this$props.tag,
        innerRef = _this$props.innerRef,
        attributes = _objectWithoutPropertiesLoose(_this$props, ["active", "aria-label", "block", "className", "close", "cssModule", "color", "outline", "size", "tag", "innerRef"]);

    if (close && typeof attributes.children === 'undefined') {
      attributes.children = /*#__PURE__*/react.createElement("span", {
        "aria-hidden": true
      }, "\xD7");
    }

    var btnOutlineColor = "btn" + (outline ? '-outline' : '') + "-" + color;
    var classes = mapToCssModules(classnames(className, {
      close: close
    }, close || 'btn', close || btnOutlineColor, size ? "btn-" + size : false, block ? 'btn-block' : false, {
      active: active,
      disabled: this.props.disabled
    }), cssModule);

    if (attributes.href && Tag === 'button') {
      Tag = 'a';
    }

    var defaultAriaLabel = close ? 'Close' : null;
    return /*#__PURE__*/react.createElement(Tag, _extends({
      type: Tag === 'button' && attributes.onClick ? 'button' : undefined
    }, attributes, {
      className: classes,
      ref: innerRef,
      onClick: this.onClick,
      "aria-label": ariaLabel || defaultAriaLabel
    }));
  };

  return Button;
}(react.Component);

Button.propTypes = propTypes$7;
Button.defaultProps = defaultProps$7;

var propTypes$8 = {
  tag: tagPropType,
  'aria-label': propTypes$g.string,
  className: propTypes$g.string,
  cssModule: propTypes$g.object,
  role: propTypes$g.string,
  size: propTypes$g.string,
  vertical: propTypes$g.bool
};
var defaultProps$8 = {
  tag: 'div',
  role: 'group'
};

var ButtonGroup = function ButtonGroup(props) {
  var className = props.className,
      cssModule = props.cssModule,
      size = props.size,
      vertical = props.vertical,
      Tag = props.tag,
      attributes = _objectWithoutPropertiesLoose(props, ["className", "cssModule", "size", "vertical", "tag"]);

  var classes = mapToCssModules(classnames(className, size ? 'btn-group-' + size : false, vertical ? 'btn-group-vertical' : 'btn-group'), cssModule);
  return /*#__PURE__*/react.createElement(Tag, _extends({}, attributes, {
    className: classes
  }));
};

ButtonGroup.propTypes = propTypes$8;
ButtonGroup.defaultProps = defaultProps$8;

var propTypes$9 = {
  color: propTypes$g.string,
  pill: propTypes$g.bool,
  tag: tagPropType,
  innerRef: propTypes$g.oneOfType([propTypes$g.object, propTypes$g.func, propTypes$g.string]),
  children: propTypes$g.node,
  className: propTypes$g.string,
  cssModule: propTypes$g.object
};
var defaultProps$9 = {
  color: 'secondary',
  pill: false,
  tag: 'span'
};

var Badge = function Badge(props) {
  var className = props.className,
      cssModule = props.cssModule,
      color = props.color,
      innerRef = props.innerRef,
      pill = props.pill,
      Tag = props.tag,
      attributes = _objectWithoutPropertiesLoose(props, ["className", "cssModule", "color", "innerRef", "pill", "tag"]);

  var classes = mapToCssModules(classnames(className, 'badge', 'badge-' + color, pill ? 'badge-pill' : false), cssModule);

  if (attributes.href && Tag === 'span') {
    Tag = 'a';
  }

  return /*#__PURE__*/react.createElement(Tag, _extends({}, attributes, {
    className: classes,
    ref: innerRef
  }));
};

Badge.propTypes = propTypes$9;
Badge.defaultProps = defaultProps$9;

var propTypes$a = {
  children: propTypes$g.node,
  row: propTypes$g.bool,
  check: propTypes$g.bool,
  inline: propTypes$g.bool,
  disabled: propTypes$g.bool,
  tag: tagPropType,
  className: propTypes$g.string,
  cssModule: propTypes$g.object
};
var defaultProps$a = {
  tag: 'div'
};

var FormGroup = function FormGroup(props) {
  var className = props.className,
      cssModule = props.cssModule,
      row = props.row,
      disabled = props.disabled,
      check = props.check,
      inline = props.inline,
      Tag = props.tag,
      attributes = _objectWithoutPropertiesLoose(props, ["className", "cssModule", "row", "disabled", "check", "inline", "tag"]);

  var classes = mapToCssModules(classnames(className, row ? 'row' : false, check ? 'form-check' : 'form-group', check && inline ? 'form-check-inline' : false, check && disabled ? 'disabled' : false), cssModule);

  if (Tag === 'fieldset') {
    attributes.disabled = disabled;
  }

  return /*#__PURE__*/react.createElement(Tag, _extends({}, attributes, {
    className: classes
  }));
};

FormGroup.propTypes = propTypes$a;
FormGroup.defaultProps = defaultProps$a;

var propTypes$b = {
  children: propTypes$g.node,
  type: propTypes$g.string,
  size: propTypes$g.oneOfType([propTypes$g.number, propTypes$g.string]),
  bsSize: propTypes$g.string,
  valid: propTypes$g.bool,
  invalid: propTypes$g.bool,
  tag: tagPropType,
  innerRef: propTypes$g.oneOfType([propTypes$g.object, propTypes$g.func, propTypes$g.string]),
  plaintext: propTypes$g.bool,
  addon: propTypes$g.bool,
  className: propTypes$g.string,
  cssModule: propTypes$g.object
};
var defaultProps$b = {
  type: 'text'
};

var Input = /*#__PURE__*/function (_React$Component) {
  _inheritsLoose(Input, _React$Component);

  function Input(props) {
    var _this;

    _this = _React$Component.call(this, props) || this;
    _this.getRef = _this.getRef.bind(_assertThisInitialized(_this));
    _this.focus = _this.focus.bind(_assertThisInitialized(_this));
    return _this;
  }

  var _proto = Input.prototype;

  _proto.getRef = function getRef(ref) {
    if (this.props.innerRef) {
      this.props.innerRef(ref);
    }

    this.ref = ref;
  };

  _proto.focus = function focus() {
    if (this.ref) {
      this.ref.focus();
    }
  };

  _proto.render = function render() {
    var _this$props = this.props,
        className = _this$props.className,
        cssModule = _this$props.cssModule,
        type = _this$props.type,
        bsSize = _this$props.bsSize,
        valid = _this$props.valid,
        invalid = _this$props.invalid,
        tag = _this$props.tag,
        addon = _this$props.addon,
        plaintext = _this$props.plaintext,
        innerRef = _this$props.innerRef,
        attributes = _objectWithoutPropertiesLoose(_this$props, ["className", "cssModule", "type", "bsSize", "valid", "invalid", "tag", "addon", "plaintext", "innerRef"]);

    var checkInput = ['radio', 'checkbox'].indexOf(type) > -1;
    var isNotaNumber = new RegExp('\\D', 'g');
    var fileInput = type === 'file';
    var textareaInput = type === 'textarea';
    var selectInput = type === 'select';
    var rangeInput = type === 'range';
    var Tag = tag || (selectInput || textareaInput ? type : 'input');
    var formControlClass = 'form-control';

    if (plaintext) {
      formControlClass = formControlClass + "-plaintext";
      Tag = tag || 'input';
    } else if (fileInput) {
      formControlClass = formControlClass + "-file";
    } else if (rangeInput) {
      formControlClass = formControlClass + "-range";
    } else if (checkInput) {
      if (addon) {
        formControlClass = null;
      } else {
        formControlClass = 'form-check-input';
      }
    }

    if (attributes.size && isNotaNumber.test(attributes.size)) {
      warnOnce('Please use the prop "bsSize" instead of the "size" to bootstrap\'s input sizing.');
      bsSize = attributes.size;
      delete attributes.size;
    }

    var classes = mapToCssModules(classnames(className, invalid && 'is-invalid', valid && 'is-valid', bsSize ? "form-control-" + bsSize : false, formControlClass), cssModule);

    if (Tag === 'input' || tag && typeof tag === 'function') {
      attributes.type = type;
    }

    if (attributes.children && !(plaintext || type === 'select' || typeof Tag !== 'string' || Tag === 'select')) {
      warnOnce("Input with a type of \"" + type + "\" cannot have children. Please use \"value\"/\"defaultValue\" instead.");
      delete attributes.children;
    }

    return /*#__PURE__*/react.createElement(Tag, _extends({}, attributes, {
      ref: innerRef,
      className: classes,
      "aria-invalid": invalid
    }));
  };

  return Input;
}(react.Component);

Input.propTypes = propTypes$b;
Input.defaultProps = defaultProps$b;

/**
 * TabContext
 * {
 *  activeTabId: PropTypes.any
 * }
 */

var TabContext = /*#__PURE__*/react.createContext({});

var propTypes$c = {
  tag: tagPropType,
  activeTab: propTypes$g.any,
  className: propTypes$g.string,
  cssModule: propTypes$g.object
};
var defaultProps$c = {
  tag: 'div'
};

var TabContent = /*#__PURE__*/function (_Component) {
  _inheritsLoose(TabContent, _Component);

  TabContent.getDerivedStateFromProps = function getDerivedStateFromProps(nextProps, prevState) {
    if (prevState.activeTab !== nextProps.activeTab) {
      return {
        activeTab: nextProps.activeTab
      };
    }

    return null;
  };

  function TabContent(props) {
    var _this;

    _this = _Component.call(this, props) || this;
    _this.state = {
      activeTab: _this.props.activeTab
    };
    return _this;
  }

  var _proto = TabContent.prototype;

  _proto.render = function render() {
    var _this$props = this.props,
        className = _this$props.className,
        cssModule = _this$props.cssModule,
        Tag = _this$props.tag;
    var attributes = omit(this.props, Object.keys(propTypes$c));
    var classes = mapToCssModules(classnames('tab-content', className), cssModule);
    return /*#__PURE__*/react.createElement(TabContext.Provider, {
      value: {
        activeTabId: this.state.activeTab
      }
    }, /*#__PURE__*/react.createElement(Tag, _extends({}, attributes, {
      className: classes
    })));
  };

  return TabContent;
}(react.Component);
TabContent.propTypes = propTypes$c;
TabContent.defaultProps = defaultProps$c;

var propTypes$d = {
  tag: tagPropType,
  className: propTypes$g.string,
  cssModule: propTypes$g.object,
  tabId: propTypes$g.any
};
var defaultProps$d = {
  tag: 'div'
};
function TabPane(props) {
  var className = props.className,
      cssModule = props.cssModule,
      tabId = props.tabId,
      Tag = props.tag,
      attributes = _objectWithoutPropertiesLoose(props, ["className", "cssModule", "tabId", "tag"]);

  var getClasses = function getClasses(activeTabId) {
    return mapToCssModules(classnames('tab-pane', className, {
      active: tabId === activeTabId
    }), cssModule);
  };

  return /*#__PURE__*/react.createElement(TabContext.Consumer, null, function (_ref) {
    var activeTabId = _ref.activeTabId;
    return /*#__PURE__*/react.createElement(Tag, _extends({}, attributes, {
      className: getClasses(activeTabId)
    }));
  });
}
TabPane.propTypes = propTypes$d;
TabPane.defaultProps = defaultProps$d;

var propTypes$e = {
  tag: tagPropType,
  fluid: propTypes$g.bool,
  className: propTypes$g.string,
  cssModule: propTypes$g.object
};
var defaultProps$e = {
  tag: 'div'
};

var Jumbotron = function Jumbotron(props) {
  var className = props.className,
      cssModule = props.cssModule,
      Tag = props.tag,
      fluid = props.fluid,
      attributes = _objectWithoutPropertiesLoose(props, ["className", "cssModule", "tag", "fluid"]);

  var classes = mapToCssModules(classnames(className, 'jumbotron', fluid ? 'jumbotron-fluid' : false), cssModule);
  return /*#__PURE__*/react.createElement(Tag, _extends({}, attributes, {
    className: classes
  }));
};

Jumbotron.propTypes = propTypes$e;
Jumbotron.defaultProps = defaultProps$e;

var propTypes$f = {
  tag: tagPropType,
  type: propTypes$g.string,
  size: propTypes$g.string,
  color: propTypes$g.string,
  className: propTypes$g.string,
  cssModule: propTypes$g.object,
  children: propTypes$g.string
};
var defaultProps$f = {
  tag: 'div',
  type: 'border',
  children: 'Loading...'
};

var Spinner = function Spinner(props) {
  var className = props.className,
      cssModule = props.cssModule,
      type = props.type,
      size = props.size,
      color = props.color,
      children = props.children,
      Tag = props.tag,
      attributes = _objectWithoutPropertiesLoose(props, ["className", "cssModule", "type", "size", "color", "children", "tag"]);

  var classes = mapToCssModules(classnames(className, size ? "spinner-" + type + "-" + size : false, "spinner-" + type, color ? "text-" + color : false), cssModule);
  return /*#__PURE__*/react.createElement(Tag, _extends({
    role: "status"
  }, attributes, {
    className: classes
  }), children && /*#__PURE__*/react.createElement("span", {
    className: mapToCssModules('sr-only', cssModule)
  }, children));
};

Spinner.propTypes = propTypes$f;
Spinner.defaultProps = defaultProps$f;

export { Badge, Button, ButtonGroup, Col, FormGroup, Input, Jumbotron, Nav, NavItem, NavLink, Navbar, NavbarBrand, Row, Spinner, TabContent, TabPane };
