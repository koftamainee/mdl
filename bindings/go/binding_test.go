package tree_sitter_mdl_test

import (
	"testing"

	tree_sitter "github.com/tree-sitter/go-tree-sitter"
	tree_sitter_mdl "github.com/koftamainee/mdl/bindings/go"
)

func TestCanLoadGrammar(t *testing.T) {
	language := tree_sitter.NewLanguage(tree_sitter_mdl.Language())
	if language == nil {
		t.Errorf("Error loading Mantle Data Language grammar")
	}
}
