;;; mdl-ts-mode.el --- tree-sitter support for Mantle Data Language  -*- lexical-binding: t; -*-

;; Copyright (C) 2026 koftamainee

;; Author     : koftamainee <dev@koftamainee.ru>
;; Maintainer : koftamainee <dev@koftamainee.ru>
;; Created    : August 2026
;; Keywords   : languages data tools tree-sitter
;; Homepage   : https://github.com/koftamainee/mdl
;; Package-Requires: ((emacs "29.1"))

;; This file is not part of GNU Emacs.

;; This program is free software: you can redistribute it and/or modify
;; it under the terms of the GNU General Public License as published by
;; the Free Software Foundation, either version 3 of the License, or
;; (at your option) any later version.

;; This program is distributed in the hope that it will be useful,
;; but WITHOUT ANY WARRANTY; without even the implied warranty of
;; MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
;; GNU General Public License for more details.

;; You should have received a copy of the GNU General Public License
;; along with GNU Emacs.  If not, see <https://www.gnu.org/licenses/>.

;;; Commentary:

;; Tree-sitter based major mode for editing MDL (Mantle Data Language)
;; files, as used by the Mantle Game Engine.

;; MDL is a data declaration language with a JSON-like, but more
;; verbose, syntax:
;;     shader pbr
;;     stages {
;;       vertex { src shaders/pbr.slang }
;;     }

;; It provides syntax highlighting, indentation for object and list
;; bodies, structure navigation (pair/object as defuns), and an imenu
;; index built from project keys.

;; Indentation follows `mdl-ts-indent-offset', falling back to
;; `tab-width'.  This lets EditorConfig (as used by Doom Emacs) control
;; indentation through the standard `editorconfig-indentation-alist'
;; hook, which sets `tab-width' and friends per buffer.

;;; Code:

(require 'treesit)
(eval-when-compile (require 'rx))
(treesit-declare-unavailable-functions)

(add-to-list
 'treesit-language-source-alist
 '(mdl "https://github.com/koftamainee/mdl")
 t)

(defgroup mdl-ts nil
  "Major mode for editing MDL files with tree-sitter."
  :prefix "mdl-ts-"
  :group 'languages)

(defcustom mdl-ts-indent-offset nil
  "Indentation offset for MDL object/list bodies.
If nil, `tab-width' (which EditorConfig sets per buffer) is used."
  :type '(choice (const :tag "Use tab-width" nil)
                 (integer :tag "Offset"))
  :group 'mdl-ts)

(defvar mdl-ts-mode--syntax-table
  (let ((table (make-syntax-table)))
    (modify-syntax-entry ?#  "<" table)
    (modify-syntax-entry ?\n ">" table)
    table)
  "Syntax table for `mdl-ts-mode'.")

(defun mdl-ts-mode--closing-bracket-p ()
  "Return a predicate for lines ending a block.
True when the first non-whitespace character on the line is a
closing bracket (`}' or `]')."
  (lambda (_node _parent bol)
    (save-excursion
      (goto-char bol)
      (skip-chars-forward " \t")
      (looking-at "[}\]]"))))

(defvar mdl-ts-mode--indent-rules
  `((mdl
     ((mdl-ts-mode--closing-bracket-p) parent-bol 0)
     ((parent-is "object") parent-bol ,(lambda (_n _p _b) (or mdl-ts-indent-offset tab-width)))
     ((parent-is "list") parent-bol ,(lambda (_n _p _b) (or mdl-ts-indent-offset tab-width)))))
  "Tree-sitter indent rules.")

(defvar mdl-ts-mode--font-lock-settings
  (treesit-font-lock-rules
   :language 'mdl
   :feature 'key
   '((key) @font-lock-keyword-face)

   :language 'mdl
   :feature 'string
   '((string) @font-lock-string-face
     (raw_string) @font-lock-string-face
     (bare_string) @font-lock-string-face)

   :language 'mdl
   :feature 'constant
   '((number) @font-lock-constant-face
     (boolean) @font-lock-constant-face)

   :language 'mdl
   :feature 'comment
   '((comment) @font-lock-comment-face))
  "Tree-sitter font-lock settings.")

(defun mdl-ts-mode--imenu-name (node)
  "Return the imenu name for a `pair' NODE: its key text.
Return nil if NODE is not a pair or has no key."
  (when (string= (treesit-node-type node) "pair")
    (treesit-node-text (car (treesit-node-children node)) t)))

(defun mdl-ts-mode--which-func (node)
  "Return the MDL project name for NODE, if any.
Used by `which-func'."
  (when (string= (treesit-node-type node) "pair")
    (mdl-ts-mode--imenu-name node)))

;;;###autoload
(define-derived-mode mdl-ts-mode prog-mode "MDL"
  "Major mode for editing MDL files, powered by tree-sitter."
  :group 'mdl-ts
  :syntax-table mdl-ts-mode--syntax-table

  (when (and (treesit-ensure-installed 'mdl)
             (treesit-ready-p 'mdl))
    (setq treesit-primary-parser (treesit-parser-create 'mdl))

    ;; Comments.
    (setq-local comment-start "# ")
    (setq-local comment-end "")
    (setq-local comment-start-skip (rx "#" (* (syntax whitespace))))
    (setq-local comment-start-line-regexp comment-start-skip)

    ;; Indent.
    (setq-local treesit-simple-indent-rules
                mdl-ts-mode--indent-rules)

    ;; Navigation.
    (setq-local treesit-thing-settings
                `((mdl
                   (sentence "object" "list" "pair"))))
    (setq-local treesit-defun-type-regexp
                (rx (or "object" "pair") (or "" "_")))

    ;; Imenu.
    (setq-local treesit-simple-imenu-settings
                `((nil "pair" nil mdl-ts-mode--imenu-name)))
    (setq-local which-func-functions
                (list #'mdl-ts-mode--which-func))

    ;; Font-lock.
    (setq-local treesit-font-lock-settings
                mdl-ts-mode--font-lock-settings)
    (setq-local treesit-font-lock-feature-list
                '((comment)
                  (key)
                  (string constant)
                  (comment)))

    (treesit-major-mode-setup)))

;;;###autoload
(defun mdl-ts-mode-maybe ()
  "Enable `mdl-ts-mode' when its grammar is available.
Also propose to install the grammar when `treesit-enabled-modes'
is t or contains the mode name."
  (declare-function treesit-language-available-p "treesit.c")
  (if (or (treesit-language-available-p 'mdl)
          (eq treesit-enabled-modes t)
          (memq 'mdl-ts-mode treesit-enabled-modes))
      (mdl-ts-mode)
    (fundamental-mode)))

;;;###autoload
(when (boundp 'treesit-major-mode-remap-alist)
  (add-to-list 'auto-mode-alist '("\\.mdl\\'" . mdl-ts-mode-maybe)))

(provide 'mdl-ts-mode)

;;; mdl-ts-mode.el ends here
